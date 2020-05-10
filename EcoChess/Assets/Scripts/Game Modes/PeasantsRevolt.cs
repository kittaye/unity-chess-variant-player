
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     . n n . k . n .
    ///     . . . . p . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     . . . . K . . .
    /// </summary>
    public class PeasantsRevolt : Chess {
        public PeasantsRevolt() : base() { }

        public override string ToString() {
            return "Peasants' Revolt";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by George Whelon",
                this.ToString() + " is a variant with a standard army of pawns for white and 3 knights and a pawn for black.",
                "Checkmate.",
                VariantHelpDetails.rule_None,
                "https://www.chessvariants.com/large.dir/peasantrevolt.html"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(4, BLACK_PAWNROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));

                if (x == 1 || x == 2 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }
    }
}
