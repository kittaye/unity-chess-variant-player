
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     n n n n k n n n
    ///     p p p p p p p p
    ///     . . p . . p . .
    ///     . p p p p p p .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     R N B Q K B N R
    /// </summary>
    public class Weak : Chess {
        private Piece[] whitePromotionOptions;
        private Piece[] blackPromotionOptions;

        public Weak() : base() {
            whitePromotionOptions = PawnPromotionOptions;
            blackPromotionOptions = new Piece[0];
        }

        public override string ToString() {
            return "Weak!";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented in the 1960s",
                this.ToString() + " is a variant that pits the FIDE standard white army against a custom black army of knights and pawns.",
                "Checkmate.",
                "- Black pawns may only promote to a knight.",
                "https://www.chessvariants.com/unequal.dir/weak.html"
            );
        }

        public override void OnMoveComplete() {
            base.OnMoveComplete();
            if(GetCurrentTeamTurn() == Team.WHITE) {
                SelectedPawnPromotion = Piece.Queen;
                PawnPromotionOptions = whitePromotionOptions;
            } else {
                SelectedPawnPromotion = Piece.Knight;
                PawnPromotionOptions = blackPromotionOptions;
            }
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, "d1");
            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, "c1");
            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, "f1");
            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, "b1");
            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, "g1");
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, "a1");
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, "h1");

            ((Pawn)AddNewPieceToBoard(Piece.Pawn, Team.BLACK, "c6")).initialMoveLimit = 1;
            ((Pawn)AddNewPieceToBoard(Piece.Pawn, Team.BLACK, "f6")).initialMoveLimit = 1;

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (x != 4) {
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
                if (x >= 1 && x <= BOARD_WIDTH - 2) {
                    ((Pawn)AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW - 2))).initialMoveLimit = 1;
                }
            }
        }
    }
}
