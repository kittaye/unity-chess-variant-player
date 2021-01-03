
namespace ChessGameModes {
    /// <summary>
    /// Board layout: FIDE standard.
    /// </summary>
    public class KingOfTheHill : Chess {
        private readonly BoardCoord CENTER_SQUARE_1 = new BoardCoord(3, 3);
        private readonly BoardCoord CENTER_SQUARE_2 = new BoardCoord(3, 4);
        private readonly BoardCoord CENTER_SQUARE_3 = new BoardCoord(4, 3);
        private readonly BoardCoord CENTER_SQUARE_4 = new BoardCoord(4, 4);

        public KingOfTheHill() : base(BOARD_WIDTH, BOARD_HEIGHT) { }

        public override string ToString() {
            return "King of the Hill";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by ???",
                this.ToString() + " is a variant that includes an additional win condition.",
                "Checkmate, or move a king onto one of the four central squares (d4, d5, e4, e5).",
                "- Note: The king's move onto a central square must be legal (i.e. must not move into check).",
                "https://lichess.org/variant/kingOfTheHill"
            );
        }

        public override bool CheckWinState() {
            if (opposingRoyalPiece.GetBoardPosition() == CENTER_SQUARE_1 || opposingRoyalPiece.GetBoardPosition() == CENTER_SQUARE_2
                || opposingRoyalPiece.GetBoardPosition() == CENTER_SQUARE_3 || opposingRoyalPiece.GetBoardPosition() == CENTER_SQUARE_4) {
                UIManager.Instance.LogCustom("Team " + GetOpposingTeamTurn().ToString() + " has reached the center! -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
                return true;
            }

            return base.CheckWinState();
        }
    }
}
