using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    public class DummyVariant : Chess {
        private new const int BOARD_WIDTH = 12;
        private new const int BOARD_HEIGHT = 12;

        public DummyVariant() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            Board.RemoveBoardCoordinates(new string[] { "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "a10", "a11" });
            Board.RemoveBoardCoordinates(new string[] { "l2", "l3", "l4", "l5", "l6", "l7", "l8", "l9", "l10", "l11" });
            Board.RemoveBoardCoordinates(new string[] { "b1", "c1", "d1", "e1", "f1", "g1", "h1", "i1", "j1", "k1" });
            Board.RemoveBoardCoordinates(new string[] { "b12", "c12", "d12", "e12", "f12", "g12", "h12", "i12", "j12", "k12" });

            Board.SetCustomBoardCoordinateKey("a1", "W1");
            Board.SetCustomBoardCoordinateKey("l1", "W2");
            Board.SetCustomBoardCoordinateKey("a12", "W3");
            Board.SetCustomBoardCoordinateKey("l12", "W4");

            Board.ResetAlgebraicKeys("b2", 10, 10);
        }

        public override string ToString() {
            return "Dummy Variant";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, 2)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW - 2)));

            AddPieceToBoard(new Princess(Team.WHITE, new BoardCoord(3, 3)));
            AddPieceToBoard(new Empress(Team.WHITE, new BoardCoord(4, 3)));
            AddPieceToBoard(new Nightrider(Team.WHITE, new BoardCoord(5, 3)));
            AddPieceToBoard(new Grasshopper(Team.WHITE, new BoardCoord(6, 3)));
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> checkThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            GetPiecesOfType<Empress>(pieceToCheck.GetOpposingTeam()).ForEach((x) => { checkThreats.Add(x); });
            GetPiecesOfType<Princess>(pieceToCheck.GetOpposingTeam()).ForEach((x) => { checkThreats.Add(x); });
            GetPiecesOfType<Nightrider>(pieceToCheck.GetOpposingTeam()).ForEach((x) => { checkThreats.Add(x); });

            return checkThreats;
        }
    }
}
