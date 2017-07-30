using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// ReversedRoyals.cs is a chess variant with a custom initial board layout.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Board layout:
    ///     r b b k q n n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     R B B Q K N N R
    /// </summary>
    public class ReversedRoyals : FIDERuleset {
        public ReversedRoyals() : base() {
        }

        public override string ToString() {
            return "Reversed Royals Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(7, WHITE_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(3, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        protected override void TryPerformCastlingRookMoves(ChessPiece mover) {
            if (mover.GetTeam() == Team.WHITE) {
                if(mover.GetBoardPosition().x == 2) {
                    aSideWhiteRook = PerformCastle(aSideWhiteRook, new BoardCoord(3, mover.GetBoardPosition().y));
                } else if(mover.GetBoardPosition().x == 6) {
                    hSideWhiteRook = PerformCastle(hSideWhiteRook, new BoardCoord(5, mover.GetBoardPosition().y));
                }
            } else {
                if (mover.GetBoardPosition().x == 1) {
                    aSideBlackRook = PerformCastle(aSideBlackRook, new BoardCoord(2, mover.GetBoardPosition().y));
                } else if (mover.GetBoardPosition().x == 5) {
                    hSideBlackRook = PerformCastle(hSideBlackRook, new BoardCoord(4, mover.GetBoardPosition().y));
                }
            }
        }
    }
}
