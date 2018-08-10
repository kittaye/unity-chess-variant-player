using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// ActiveChess.cs is a chess variant on a 9x8 board adding an extra queen.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Board layout:
    ///     r n b k q b n r q
    ///     p p p p p p p p p
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     p p p p p p p p p
    ///     R N B Q K B N R Q
    /// </summary>
    public class ActiveChess : Chess {
        private new const int BOARD_WIDTH = 9;
        public ActiveChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
        }

        public override string ToString() {
            return "Active Chess";
        }

        public override void PopulateBoard() {
            base.PopulateBoard();

            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(BOARD_WIDTH - 1, WHITE_PAWNROW)));
            AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(BOARD_WIDTH - 1, BLACK_PAWNROW)));
            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(BOARD_WIDTH - 1, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(BOARD_WIDTH - 1, BLACK_BACKROW)));
        }
    }
}
