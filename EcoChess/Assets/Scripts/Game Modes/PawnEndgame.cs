
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     . . . . k . . .
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     . . . . K . . .
    /// </summary>
    public class PawnEndgame : Chess {
        public PawnEndgame() : base() { }

        public override string ToString() {
            return "Pawn Endgame";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by ???",
                this.ToString() + " is a variant that removes all initial pieces except the kings and pawns.",
                "Checkmate.",
                VariantHelpDetails.rule_None,
                "https://greenchess.net/rules.php?v=pawn-endgame"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));
            }
        }
    }
}
