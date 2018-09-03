using System;
using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// GrandChess.cs is a chess variant on a 10x10 board with empresses and princesses.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox + Empresses, Princesses.
    /// Piece rules: No Castling.
    /// Board layout: 
    ///     r . . . . . . . . r
    ///     . n b q k $ ^ b n .
    ///     p p p p p p p p p p
    ///     . . . . . . . . . .     $ = Empress
    ///     . . . . . . . . . .     ^ = Princess
    ///     . . . . . . . . . .
    ///     . . . . . . . . . .
    ///     p p p p p p p p p p
    ///     . N B Q K $ ^ B N .
    ///     R . . . . . . . . R
    /// </summary>
    public class GrandChess : Chess {
        private new const int BOARD_WIDTH = 10;
        private new const int BOARD_HEIGHT = 10;
        private new const int WHITE_PAWNROW = 2;

        public GrandChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            PawnPromotionOptions = new Piece[6] { Piece.Queen, Piece.Empress, Piece.Princess, Piece.Rook, Piece.Bishop, Piece.Knight };
            BLACK_PAWNROW = Board.GetHeight() - 3;
            AllowCastling = false;
        }

        public override string ToString() {
            return "Grand Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW + 1)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW - 1)));

            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(9, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(9, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(3, WHITE_BACKROW + 1)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW - 1)));

            AddPieceToBoard(new Empress(Team.WHITE, new BoardCoord(5, WHITE_BACKROW + 1)));
            AddPieceToBoard(new Empress(Team.BLACK, new BoardCoord(5, BLACK_BACKROW - 1)));

            AddPieceToBoard(new Princess(Team.WHITE, new BoardCoord(6, WHITE_BACKROW + 1)));
            AddPieceToBoard(new Princess(Team.BLACK, new BoardCoord(6, BLACK_BACKROW - 1)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW + 1)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW - 1)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW + 1)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW - 1)));
                }
            }
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            GetPiecesOfType<Empress>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });
            GetPiecesOfType<Princess>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });

            return possibleCheckThreats;
        }
    }
}