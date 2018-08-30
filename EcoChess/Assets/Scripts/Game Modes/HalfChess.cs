using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// HalfChess.cs is a chess variant with a custom board layout on a 4x8 board.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox - Pawns.
    /// Board layout:
    ///     r n . . . . N R
    ///     q b . . . . B Q
    ///     k b . . . . B K
    ///     r n . . . . N R
    /// </summary>
    public class HalfChess : Chess {
        private new const int BOARD_HEIGHT = 4;

        public HalfChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            AllowCastling = false;
            AllowEnpassantCapture = false;
        }

        public override string ToString() {
            return "Half Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, "h2"));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, "a2"));

            AddPieceToBoard(new Queen(Team.WHITE, "h3"));
            AddPieceToBoard(new Queen(Team.BLACK, "a3"));

            AddPieceToBoard(new Rook(Team.WHITE, "h1"));
            AddPieceToBoard(new Rook(Team.WHITE, "h4"));
            AddPieceToBoard(new Rook(Team.BLACK, "a1"));
            AddPieceToBoard(new Rook(Team.BLACK, "a4"));

            AddPieceToBoard(new Knight(Team.WHITE, "g1"));
            AddPieceToBoard(new Knight(Team.WHITE, "g4"));
            AddPieceToBoard(new Knight(Team.BLACK, "b1"));
            AddPieceToBoard(new Knight(Team.BLACK, "b4"));

            AddPieceToBoard(new Bishop(Team.WHITE, "g2"));
            AddPieceToBoard(new Bishop(Team.WHITE, "g3"));
            AddPieceToBoard(new Bishop(Team.BLACK, "b2"));
            AddPieceToBoard(new Bishop(Team.BLACK, "b3"));
        }
    }
}