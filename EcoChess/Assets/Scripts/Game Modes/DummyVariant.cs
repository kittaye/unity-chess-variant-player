using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    public class DummyVariant : Chess {

        public DummyVariant() : base() {
        }

        public override string ToString() {
            return "Dummy Variant";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, 2)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW - 2)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(3, 3)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(4, 3)));
        }
    }
}
