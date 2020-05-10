
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r q k r
    ///     p p p p
    ///     . . . .
    ///     p p p p
    ///     R Q K R
    /// </summary>
    public class Silverman4x5 : Chess {
        private new const uint BOARD_WIDTH = 4;
        private new const uint BOARD_HEIGHT = 5;

        public Silverman4x5() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            PawnPromotionOptions = new Piece[] { Piece.Queen, Piece.Rook };
            AllowCastling = false;
            AllowEnpassantCapture = false;
        }

        public override string ToString() {
            return "Silverman 4x5";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by David Silverman (1981)",
                this.ToString() + " is a variant on a smaller board layout (4x5).",
                "Checkmate.",
                "- Pawns may only promote to a queen or rook.\n" +
                VariantHelpDetails.rule_NoPawnDoubleMove + "\n" +
                VariantHelpDetails.rule_NoEnpassantCapture + "\n" +
                VariantHelpDetails.rule_NoCastling,
                "https://en.wikipedia.org/wiki/Minichess#4.C3.974.2C_4.C3.975_and_4.C3.978_chess"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(2, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(2, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(3, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(3, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(1, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(1, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                ((Pawn)AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW))).initialMoveLimit = 1;
                ((Pawn)AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW))).initialMoveLimit = 1;
            }
        }
    }
}
