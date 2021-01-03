using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: FIDE standard.
    /// </summary>
    public class LosingChess : Chess {
        private bool canCaptureThisTurn;

        public LosingChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            canCaptureThisTurn = false;
            AllowCastling = false;
        }

        public override string ToString() {
            return "Losing Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Walter Campbell (1874)",
                this.ToString() + " is a variant that makes piece capture compulsory.",
                "Lose all your own pieces or be stalemated.",
                "- Kings have no royalty (no check/mate rules).\n" +
                "- Pawns may also promote to a king.\n" + 
                VariantHelpDetails.rule_NoCastling,
                "https://www.chessvariants.com/diffobjective.dir/giveaway.html"
            );
        }

        public override bool CheckWinState() {
            if (GetAlivePiecesOfType<ChessPiece>(GetCurrentTeamTurn()).Count == 0) {
                UIManager.Instance.LogCustom("Team " + GetCurrentTeamTurn().ToString() + " has lost all pieces -- Team " + GetCurrentTeamTurn().ToString() + " wins!");
                return true;
            }

            canCaptureThisTurn = CanCaptureAPiece();

            if (canCaptureThisTurn) {
                return false;
            }

            if (!TeamHasAnyMoves(GetCurrentTeamTurn())) {
                UIManager.Instance.LogStalemate(GetCurrentTeamTurn().ToString());
                return true;
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            if (canCaptureThisTurn) {
                for (int i = 0; i < templateMoves.Length; i++) {
                    if (mover.IsThreatTowards(templateMoves[i])) {
                        availableMoves.Add(templateMoves[i]);
                    }
                }
            } else {
                availableMoves.AddRange(templateMoves);
            }

            if (mover is Pawn) {
                availableMoves.AddRange(TryAddAvailableEnPassantMoves((Pawn)mover));
            }

            return availableMoves;
        }

        private bool CanCaptureAPiece() {
            foreach (ChessPiece piece in GetAlivePiecesOfType<ChessPiece>(GetCurrentTeamTurn())) {
                BoardCoord[] templateMoves = piece.CalculateTemplateMoves().ToArray();
                for (int i = 0; i < templateMoves.Length; i++) {
                    if (piece.IsThreatTowards(templateMoves[i])) {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override BoardCoord[] TryAddAvailableEnPassantMoves(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;
            List<BoardCoord> enpassantMoves = new List<BoardCoord>(1);

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    BoardCoord sidewaysCoord = mover.GetRelativeBoardCoord(i, 0);

                    if (Board.ContainsCoord(sidewaysCoord) && mover.IsThreatTowards(sidewaysCoord)) {
                        ChessPiece piece = Board.GetCoordInfo(sidewaysCoord).GetAliveOccupier();

                        if (piece is Pawn && CheckEnPassantVulnerability((Pawn)piece)) {
                            BoardCoord enpassantCoord = mover.GetRelativeBoardCoord(i, 1);
                            if (Board.ContainsCoord(enpassantCoord)) {
                                enpassantMoves.Add(enpassantCoord);
                            }
                        }
                    }
                }
            }
            return enpassantMoves.ToArray();
        }
    }
}