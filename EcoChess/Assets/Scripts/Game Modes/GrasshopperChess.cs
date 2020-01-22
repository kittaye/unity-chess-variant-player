using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r n b q k b n r
    ///     g g g g g g g g
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     G G G G G G G G
    ///     R N B Q K B N R
    /// </summary>
    public class GrasshopperChess : Chess {
        private new const int WHITE_PAWNROW = 2;
        public GrasshopperChess() : base() {
            PawnPromotionOptions = new Piece[5] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight, Piece.Grasshopper };
            BLACK_PAWNROW = Board.GetHeight() - 3;
        }

        public override string ToString() {
            return "Grasshopper Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by J. Boyer (1950s)",
                this.ToString() + " is a variant including a row of grasshoppers for both teams.",
                "Checkmate.",
                "- Pawns may also promote to a grasshopper.\n" +
                "- Note: Grasshoppers move in any direction, but must be next to another piece to 'hop' over it.",
                "https://web.archive.org/web/20130425044448/http://www.chessvariants.org/dpieces.dir/grashopper.html"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(7, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(3, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW), initialMoveLimit: 1));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW), initialMoveLimit: 1));

                AddPieceToBoard(new Grasshopper(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW - 1)));
                AddPieceToBoard(new Grasshopper(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW + 1)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }
    }
}
