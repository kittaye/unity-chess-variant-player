using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// ThreeCheck.cs is a chess variant that offers an additional win condition.
    /// 
    /// Winstate: Checkmate OR Total of 3 checks against opposing king.
    /// Piece types: Orthodox.
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

        public override bool CheckWinState() {
            bool hasAnyMoves = false;
            foreach (ChessPiece piece in GetPieces(GetCurrentTeamTurn())) {
                if (piece.IsAlive) {
                    if (CalculateAvailableMoves(piece).Count > 0) {
                        hasAnyMoves = true;
                        break;
                    }
                }
            }

            if (IsPieceInCheck(currentRoyalPiece)) {
                if (currentRoyalPiece.GetTeam() == Team.WHITE) {
                    numOfChecksWHITE++;
                } else {
                    numOfChecksBLACK++;
                }

                if (numOfChecksWHITE == 3 || numOfChecksBLACK == 3) {
                    UIManager.Instance.Log("Team " + GetCurrentTeamTurn().ToString() + " has been checked 3 times -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
                    return true;
                }

                if (hasAnyMoves == false) {
                    UIManager.Instance.Log("Team " + GetCurrentTeamTurn().ToString() + " has been checkmated -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
                    return true;
                }
            }

            if (CapturelessMovesLimit()) {
                return true;
            }
            return false;
        }
    }
}