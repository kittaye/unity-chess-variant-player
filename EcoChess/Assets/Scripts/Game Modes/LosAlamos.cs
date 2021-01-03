
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r n q k n r
    ///     p p p p p p
    ///     . . . . . .
    ///     . . . . . .
    ///     p p p p p p
    ///     R N Q K N R
    /// </summary>
    public class LosAlamos : Chess {
        private new const int BOARD_WIDTH = 6;
        private new const int BOARD_HEIGHT = 6;

        public LosAlamos() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            PawnPromotionOptions = new Piece[3] { Piece.Queen, Piece.Rook, Piece.Knight };
            AllowCastling = false;
            AllowEnpassantCapture = false;
        }

        public override string ToString() {
            return "Los Alamos Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Paul Stein & Mark Wells (1956)",
                this.ToString() + " is a variant on a 6x6 board with no bishops.",
                "Checkmate.",
                "- Pawns may not promote to a bishop.\n" +
                VariantHelpDetails.rule_NoCastling + "\n" + 
                VariantHelpDetails.rule_NoPawnDoubleMove + "\n" +
                VariantHelpDetails.rule_NoEnpassantCapture,
                "https://en.wikipedia.org/wiki/Los_Alamos_chess"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, "d1");
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, "d6");

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, "c1");
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, "c6");

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(BOARD_WIDTH - 1, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(BOARD_WIDTH - 1, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                ((Pawn)AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW))).initialMoveLimit = 1;
                ((Pawn)AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW))).initialMoveLimit = 1;

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }
    }
}
