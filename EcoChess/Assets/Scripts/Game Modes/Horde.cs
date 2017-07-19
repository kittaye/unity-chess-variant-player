using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Horde.cs is a chess variant that pits 36 white pawns against standard team black.
    /// 
    /// Winstate: Checkmate team black OR Eliminate team white.
    /// Piece types: Orthodox.
    /// Board layout: 
    ///     r n b q k b n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . p p . . p p .
    ///     p p p p p p p p
    ///     p p p p p p p p
    ///     p p p p p p p p
    ///     p p p p p p p p
    /// </summary>
    public class Horde : FIDERuleset {
        public Horde() : base() { }

        public override string ToString() {
            return "Horde";
        }

        public override void OnTurnComplete() {
            currentTeamTurn = (currentTeamTurn == Team.WHITE) ? Team.BLACK : Team.WHITE;
            opposingTeamTurn = (currentTeamTurn == Team.WHITE) ? Team.BLACK : Team.WHITE;
        }

        public override bool CheckWinState() {
            if (numConsecutiveCapturelessMoves == 100) {
                Debug.Log("No captures or pawn moves in 50 turns. Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
                return true;
            }

            if (GetCurrentTeamTurn() == Team.WHITE) {
                if (GetPieces(Team.WHITE).TrueForAll((x) => (x.IsAlive == false))) {
                    Debug.Log("Team Black wins by elimination!");
                    return true;
                }
            } else {
                foreach (ChessPiece piece in GetPieces(Team.BLACK)) {
                    if (piece.IsAlive) {
                        if (CalculateAvailableMoves(piece).Count > 0) {
                            return false;
                        }
                    }
                }

                if (IsPieceInCheck(currentRoyalPiece)) {
                    Debug.Log("Team " + GetCurrentTeamTurn().ToString() + " has been checkmated -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
                } else {
                    Debug.Log("Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
                }
                return true;
            }
            return false;
        }

        public override void PopulateBoard() {
            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(1, 4), false));
            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(2, 4), false));
            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(5, 4), false));
            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(6, 4), false));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, 0)));
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, 1)));
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, 2), false));
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, 3), false));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            if (GetCurrentTeamTurn() == Team.WHITE) {
                availableMoves.AddRange(templateMoves);
            } else {
                for (int i = 0; i < templateMoves.Length; i++) {
                    if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                        availableMoves.Add(templateMoves[i]);
                    }
                }

                if (mover is King && mover.MoveCount == 0) {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover));
                } else if (mover is Pawn) {
                    BoardCoord enPassantMove = TryAddAvailableEnPassantMove((Pawn)mover);
                    if (enPassantMove != BoardCoord.NULL) {
                        availableMoves.Add(enPassantMove);
                    }
                }
            }
            return availableMoves;
        }
    }
}