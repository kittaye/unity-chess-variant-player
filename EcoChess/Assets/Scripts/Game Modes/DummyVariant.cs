using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    public class DummyVariant : FIDERuleset {
        private new const int BOARD_WIDTH = 8;
        private new const int BOARD_HEIGHT = 8;

        public DummyVariant() : base(BOARD_WIDTH, BOARD_HEIGHT) { }

        public override string ToString() {
            return "Dummy Variant";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

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
