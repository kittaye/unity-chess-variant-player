using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// SovereignChess.cs is a chess variant involving multi-coloured armies to control.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Board layout:
    ///     r n b q k b n r r n b q k b n r
    ///     p p p p p p p p p p p p p p p p
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     p p p p p p p p p p p p p p p p
    ///     R N B Q K B N R R N B Q K B N R
    /// </summary>
    public class SovereignChess : FIDERuleset {
        private new const int BOARD_WIDTH = 16;
        private new const int BOARD_HEIGHT = 16;

        public SovereignChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            board.GetCoordInfo(new BoardCoord(BOARD_WIDTH / 2, BOARD_HEIGHT / 2)).boardChunk.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        public override string ToString() {
            return "Sovereign Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(8, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(8, BLACK_BACKROW)));

            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(11, WHITE_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(11, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(7, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                if (x > 3 && x < 12) {
                    AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                    AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));
                }

                if (x == 5 || x == 10) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 6 || x == 9) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }
    }
}
