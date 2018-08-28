﻿using System;
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
    public class KingOfTheHill : Chess {
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
                UIManager.Instance.LogCustom("Team " + GetOpposingTeamTurn().ToString() + " has reached the center! -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
            }

            if (!TeamHasAnyMoves(GetCurrentTeamTurn())) {
                if (IsPieceInCheck(currentRoyalPiece)) {
                    UIManager.Instance.LogCheckmate(GetOpposingTeamTurn().ToString(), GetCurrentTeamTurn().ToString());
                } else {
                    UIManager.Instance.LogStalemate(GetCurrentTeamTurn().ToString());
                }
                return true;
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }
    }
}
