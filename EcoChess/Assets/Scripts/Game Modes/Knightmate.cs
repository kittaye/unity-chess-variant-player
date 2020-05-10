﻿
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r k b q n b k r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     R K B Q N B K R
    /// </summary>
    public class Knightmate : Chess {
        public Knightmate() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            PawnPromotionOptions = new Piece[4] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.King };
        }

        public override string ToString() {
            return "Knightmate";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Bruce Zimov (1972)",
                this.ToString() + " is a variant where the roles and placements of the king and knights are reversed.",
                "Checkmate the knight.",
                "- Pawns may not promote to a knight, but may promote to a king.\n" +
                "- The knight may castle with the rooks under the same castling rules of orthodox chess.\n" +
                "- Note: The knight now has royalty, meaning it can be checked.\n" +
                "- Note: Kings are no longer royal and may be captured like other non-royal piece.",
                "https://www.chessvariants.com/diffobjective.dir/knightmate.html"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (Knight)AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (Knight)AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(7, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(7, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(3, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(3, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }
    }
}
