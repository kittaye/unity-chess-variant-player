
namespace ChessGameModes {
    /// <summary>
    /// Board layout: FIDE standard.
    /// </summary>
    public class ThreeCheck : Chess {
        private int numOfChecksWHITE;
        private int numOfChecksBLACK;

        public ThreeCheck() : base() {
            numOfChecksWHITE = 0;
            numOfChecksBLACK = 0;
        }

        public override string ToString() {
            return "Three-Check Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by ???",
                this.ToString() + " is a variant that offers an additional way to win: check the opponent's king three times.",
                "Checkmate, or check the opponent's king a total of three times.",
                VariantHelpDetails.rule_None,
                "https://lichess.org/variant/threeCheck"
            );
        }

        public override bool CheckWinState() {
            bool hasAnyMoves = TeamHasAnyMoves(GetCurrentTeamTurn());

            if (IsPieceInCheck(currentRoyalPiece)) {
                if (currentRoyalPiece.GetTeam() == Team.WHITE) {
                    numOfChecksWHITE++;
                } else {
                    numOfChecksBLACK++;
                }

                if (!hasAnyMoves) {
                    UIManager.Instance.LogCheckmate(GetOpposingTeamTurn().ToString(), GetCurrentTeamTurn().ToString());
                    return true;

                } else if (numOfChecksWHITE == 3 || numOfChecksBLACK == 3) {
                    UIManager.Instance.LogCustom("Team " + GetCurrentTeamTurn().ToString() + " has been checked 3 times -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
                    return true;

                }
            }

            if (!hasAnyMoves) {
                UIManager.Instance.LogStalemate(GetCurrentTeamTurn().ToString());
                return true;
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }
    }
}