
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r b b q k b b r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .     $ = Empress
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     R N N $ K N N R
    /// </summary>
    public class Chigorin : Chess {
        private Piece[] whitePromotionOptions;
        private Piece[] blackPromotionOptions;

        public Chigorin() : base() {
            whitePromotionOptions = new Piece[] { Piece.Empress, Piece.Knight };
            blackPromotionOptions = new Piece[] { Piece.Queen, Piece.Bishop };
        }

        public override string ToString() {
            return "Chigorin Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Ralph Betza (2002)",
                this.ToString() + " is a variant with knights and an empress for team white; bishops and a queen for team black.",
                "Checkmate.",
                "- White pawns may only promote to an empress or knight.\n" +
                "- Black pawns may only promote to a queen or bishop.",
                "https://www.chessvariants.com/diffsetup.dir/chigorin.html"
            );
        }

        public override void OnMoveComplete() {
            base.OnMoveComplete();
            if (GetCurrentTeamTurn() == Team.WHITE) {
                SelectedPawnPromotion = Piece.Empress;
                PawnPromotionOptions = whitePromotionOptions;
            } else {
                SelectedPawnPromotion = Piece.Queen;
                PawnPromotionOptions = blackPromotionOptions;
            }
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(7, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(7, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(1, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(2, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(5, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(6, WHITE_BACKROW));

            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(1, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(2, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(5, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(6, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Empress, Team.WHITE, new BoardCoord(3, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(3, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));
            }
        }
    }
}
