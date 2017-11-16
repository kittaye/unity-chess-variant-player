using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// DoubleChess.cs is a chess variant with a larger board size and 2 kings.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Piece rules: A-side king castles a-side, p-side castles p-side. Pawns can move up to 4 squares on first move.
    /// Board layout:
    ///     r n b q k b n r r n b q k b n r
    ///     p p p p p p p p p p p p p p p p
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     p p p p p p p p p p p p p p p p
    ///     R N B Q K B N R R N B Q K B N R
    /// </summary>
    public class DoubleChess : FIDERuleset {
        private new const int BOARD_WIDTH = 16;
        private new const int BOARD_HEIGHT = 12;
        private King secondCurrentKing;
        private King secondOpposingKing;

        public DoubleChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            secondCurrentKing = secondOpposingKing = null;
        }

        public override string ToString() {
            return "Double-Chess";
        }

        public override void OnTurnComplete() {
            base.OnTurnComplete();

            King temp = secondCurrentKing;
            secondCurrentKing = secondOpposingKing;
            secondOpposingKing = temp;
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));
            secondCurrentKing = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(12, WHITE_BACKROW)));
            secondOpposingKing = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(12, BLACK_BACKROW)));

            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(15, WHITE_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(15, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW), initialMoveLimit: 4));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW), initialMoveLimit: 4));

                if (x == 1 || x == 6 || x == 9 || x == 14) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == 5 || x == 10 || x == 13) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 3 || x == 11) {
                    AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 7 || x == 8) {
                    AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        public override bool CheckWinState() {
            foreach (ChessPiece piece in GetPieces(GetCurrentTeamTurn())) {
                if (piece.IsAlive) {
                    if (CalculateAvailableMoves(piece).Count > 0) return false;
                }
            }

            if (IsPieceInCheck(currentRoyalPiece) || IsPieceInCheck(secondCurrentKing)) {
                UIManager.Instance.Log("Team " + GetCurrentTeamTurn().ToString() + " has been checkmated -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
            } else {
                UIManager.Instance.Log("Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
            }
            return true;
        }

        protected override void TryPerformCastlingRookMoves(ChessPiece mover) {
            if (mover.GetBoardPosition().x == 2) {
                if (mover.GetTeam() == Team.WHITE) {
                    aSideWhiteRook = (Rook)PerformCastle(aSideWhiteRook, new BoardCoord(3, mover.GetBoardPosition().y));
                } else {
                    aSideBlackRook = (Rook)PerformCastle(aSideBlackRook, new BoardCoord(3, mover.GetBoardPosition().y));
                }
            } else if (mover.GetBoardPosition().x == 14) {
                if (mover.GetTeam() == Team.WHITE) {
                    hSideWhiteRook = (Rook)PerformCastle(hSideWhiteRook, new BoardCoord(13, mover.GetBoardPosition().y));
                } else {
                    hSideBlackRook = (Rook)PerformCastle(hSideBlackRook, new BoardCoord(13, mover.GetBoardPosition().y));
                }
            }
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false
                    && IsPieceInCheckAfterThisMove(secondCurrentKing, mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            if (mover is King && mover.MoveCount == 0) {
                if (mover == currentRoyalPiece) {
                    availableMoves.AddRange(TryAddAvailableCastleMoves((King)mover, true, false));
                } else if (mover == secondCurrentKing) {
                    availableMoves.AddRange(TryAddAvailableCastleMoves((King)mover, false, true));
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

        protected override BoardCoord TryAddAvailableEnPassantMove(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    int y = 0;
                    while(board.ContainsCoord(mover.GetRelativeBoardCoord(i, y))) {
                        BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, y));
                        if (board.ContainsCoord(coord)) {
                            ChessPiece piece = board.GetCoordInfo(coord).occupier;
                            if (piece != null) {
                                if (piece is Pawn && piece == LastMovedOpposingPiece(mover) && ((Pawn)piece).validEnPassant) {
                                    if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(i, 1)) == false) {
                                        return TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 1));
                                    }
                                } else {
                                    break;
                                }
                            }
                        }
                        y--;
                    }
                }
            }
            return BoardCoord.NULL;
        }

        protected override Pawn CheckPawnEnPassantCapture(Pawn mover) {
            int y = -1;
            while(board.ContainsCoord(mover.GetRelativeBoardCoord(0, y))) {
                ChessPiece occupier = board.GetCoordInfo(mover.GetRelativeBoardCoord(0, y)).occupier;

                if(occupier != null) {
                    if (IsThreat(mover, occupier.GetBoardPosition())) {
                        if (occupier is Pawn && ((Pawn)occupier).validEnPassant) {
                            mover.CaptureCount++;
                            RemovePieceFromBoard(occupier);
                            return (Pawn)occupier;
                        } else {
                            return null;
                        }
                    } else {
                        return null;
                    }
                }
                y--;
            }
            return null;
        }
    }
}
