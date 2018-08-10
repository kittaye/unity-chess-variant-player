using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// UpsideDown.cs is a chess variant that swaps the teams' positions with each other (pawns one move away from promoting).
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Board layout:
    ///     R N B Q K B N R
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     r n b q k b n r
    /// </summary>
    public class UpsideDown : Chess {
        public UpsideDown() : base() { }

        public override string ToString() {
            return "Upside-Down Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(3, BLACK_BACKROW)));

            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, WHITE_BACKROW)));
            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, BLACK_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, WHITE_BACKROW)));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(7, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }
    }
}