using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// ShiftedChess.cs is a chess variant with an irregular board shape.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Board layout:
    ///           q b n r
    ///   r b n k p p p p
    ///   p p p p . . . .
    ///   . . . . . . . .
    ///   . . . .   . . . .
    ///     . . . . . . . .
    ///     . . . . p p p p
    ///     p p p p K B N R
    ///     R B N Q 
    /// </summary>  
    public class ShiftedChess : FIDERuleset {
        private new const int BOARD_WIDTH = 9;
        private new const int BOARD_HEIGHT = 9;

        public ShiftedChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            board.RemoveBoardCoordinates(new string[]
            { "a1", "a2", "a3", "a4", "a9",
              "b9", "c9", "d9",
              "e5",
              "f1", "g1", "h1",
              "i1", "i6", "i7", "i8", "i9",
            });
        }

        public override string ToString() {
            return "Shifted Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, "f2"));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, "d8"));

            AddPieceToBoard(new Queen(Team.WHITE, "e1"));
            AddPieceToBoard(new Queen(Team.BLACK, "e9"));

            AddPieceToBoard(new Bishop(Team.WHITE, "c1"));
            AddPieceToBoard(new Bishop(Team.BLACK, "b8"));
            AddPieceToBoard(new Bishop(Team.WHITE, "g2"));
            AddPieceToBoard(new Bishop(Team.BLACK, "f9"));

            AddPieceToBoard(new Knight(Team.WHITE, "d1"));
            AddPieceToBoard(new Knight(Team.BLACK, "c8"));
            AddPieceToBoard(new Knight(Team.WHITE, "h2"));
            AddPieceToBoard(new Knight(Team.BLACK, "g9"));

            AddPieceToBoard(new Rook(Team.WHITE, "b1"));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, "a8"));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, "i2"));
            AddPieceToBoard(new Rook(Team.BLACK, "h9"));

            AddPieceToBoard(new Pawn(Team.WHITE, "b2"));
            AddPieceToBoard(new Pawn(Team.WHITE, "c2"));
            AddPieceToBoard(new Pawn(Team.WHITE, "d2"));
            AddPieceToBoard(new Pawn(Team.WHITE, "e2"));
            AddPieceToBoard(new Pawn(Team.WHITE, "f3"));
            AddPieceToBoard(new Pawn(Team.WHITE, "g3"));
            AddPieceToBoard(new Pawn(Team.WHITE, "h3"));
            AddPieceToBoard(new Pawn(Team.WHITE, "i3"));

            AddPieceToBoard(new Pawn(Team.BLACK, "a7"));
            AddPieceToBoard(new Pawn(Team.BLACK, "b7"));
            AddPieceToBoard(new Pawn(Team.BLACK, "c7"));
            AddPieceToBoard(new Pawn(Team.BLACK, "d7"));
            AddPieceToBoard(new Pawn(Team.BLACK, "e8"));
            AddPieceToBoard(new Pawn(Team.BLACK, "f8"));
            AddPieceToBoard(new Pawn(Team.BLACK, "g8"));
            AddPieceToBoard(new Pawn(Team.BLACK, "h8"));
        }

        protected override bool CanPromote(Pawn mover, BoardCoord[] availableMoves) {
            for (int i = 0; i < availableMoves.Length; i++) {
                if (availableMoves[i].x <= 3) {
                    if (availableMoves[i].y == WHITE_BACKROW || availableMoves[i].y == BLACK_BACKROW - 1) {
                        return true;
                    }
                } else if (availableMoves[i].x >= 5) {
                    if (availableMoves[i].y == WHITE_BACKROW + 1 || availableMoves[i].y == BLACK_BACKROW) {
                        return true;
                    }
                } else {
                    if (availableMoves[i] == new BoardCoord(4, WHITE_BACKROW) || availableMoves[i] == new BoardCoord(4, BLACK_BACKROW)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            if (mover is King && mover.MoveCount == 0) {
                if (mover.GetTeam() == Team.WHITE) {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, false, true));
                } else {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, true, false));
                }
            } else if (mover is Pawn) {
                BoardCoord enPassantMove = TryAddAvailableEnPassantMove((Pawn)mover);
                if (enPassantMove != BoardCoord.NULL) {
                    availableMoves.Add(enPassantMove);
                }
                if (checkingForCheck == false && CanPromote((Pawn)mover, availableMoves.ToArray())) {
                    OnDisplayPromotionUI(true);
                }
            }

            return availableMoves;
        }

        protected override void PerformCastlingRookMove(ChessPiece mover) {
            if (mover.GetBoardPosition().x == 1) {
                aSideBlackRook = (Rook)PerformCastle(aSideBlackRook, new BoardCoord(2, mover.GetBoardPosition().y));
            } else if (mover.GetBoardPosition().x == 7) {
                hSideWhiteRook = (Rook)PerformCastle(hSideWhiteRook, new BoardCoord(6, mover.GetBoardPosition().y));
            }
        }
    }
}