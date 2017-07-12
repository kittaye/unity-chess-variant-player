﻿using System;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// ChargeOfTheLightBrigade.cs is a chess variant with a custom initial board layout.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Board layout:
    ///     n n n n k n n n
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     . Q . Q K Q . .
    /// </summary>
    public class ChargeOfTheLightBrigade : FIDERuleset {
        public ChargeOfTheLightBrigade() : base() { }

        public override string ToString() {
            return "Charge of the Light Brigade";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if(x != 4) {
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }

                if (x == 1 || x == 3 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                }
            }
        }
    }
}
