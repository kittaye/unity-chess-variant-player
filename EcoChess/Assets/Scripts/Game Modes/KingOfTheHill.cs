using System;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// KingOfTheHill.cs is a variant of chess that includes an additional winstate.
    /// 
    /// Winstate: Checkmate OR King reaches one of the four center squares.
    /// Piece types: Orthodox.
    /// Board layout:
    ///     r n b q k b n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     R N B Q K B N R
    /// </summary>
    public class KingOfTheHill : FIDERuleset {
        private readonly BoardCoord CENTER_SQUARE_1 = new BoardCoord(3, 3);
        private readonly BoardCoord CENTER_SQUARE_2 = new BoardCoord(3, 4);
        private readonly BoardCoord CENTER_SQUARE_3 = new BoardCoord(4, 3);
        private readonly BoardCoord CENTER_SQUARE_4 = new BoardCoord(4, 4);

        public KingOfTheHill() : base(BOARD_WIDTH, BOARD_HEIGHT) { }

        public override string ToString() {
            return "King of the Hill";
        }

        public override bool CheckWinState() {
            if (opposingRoyalPiece.GetBoardPosition() == CENTER_SQUARE_1 || opposingRoyalPiece.GetBoardPosition() == CENTER_SQUARE_2
                || opposingRoyalPiece.GetBoardPosition() == CENTER_SQUARE_3 || opposingRoyalPiece.GetBoardPosition() == CENTER_SQUARE_4) {
                UIManager.Instance.Log("Team " + GetOpposingTeamTurn().ToString() + " has reached the center! -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
            }

            if (numConsecutiveCapturelessMoves == 100) {
                UIManager.Instance.Log("No captures or pawn moves in 50 turns. Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
                return true;
            }

            foreach (ChessPiece piece in GetPieces(GetCurrentTeamTurn())) {
                if (piece.IsAlive) {
                    if (CalculateAvailableMoves(piece).Count > 0) return false;
                }
            }

            if (IsPieceInCheck(currentRoyalPiece)) {
                UIManager.Instance.Log("Team " + GetCurrentTeamTurn().ToString() + " has been checkmated -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
            } else {
                UIManager.Instance.Log("Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
            }
            return true;
        }
    }
}
