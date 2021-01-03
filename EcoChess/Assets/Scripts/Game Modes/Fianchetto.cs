
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     b n r q k r n b
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     B N R Q K R N B
    /// </summary>
    public class Fianchetto : Chess {
        public Fianchetto() : base() {
            AllowCastling = false;
        }

        public override string ToString() {
            return "Fianchetto Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Jack Middleman (1999)",
                this.ToString() + " is a variant with a custom initial board layout.",
                "Checkmate.",
                VariantHelpDetails.rule_NoCastling,
                "https://www.chessvariants.com/diffsetup.dir/fianchetto.html"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(3, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(3, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (x == 0 || x == BOARD_WIDTH - 1) {
                    AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } 
            }
        }
    }
}
