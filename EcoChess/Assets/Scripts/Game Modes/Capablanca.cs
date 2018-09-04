using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Capablanca.cs is a chess variant on a 10x8 board with empresses and princesses.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox + Empresses, Princesses.
    /// Piece rules: King moves 3 squares when castling.
    /// Board layout: 
    ///     r n b $ q k ^ b n r
    ///     p p p p p p p p p p
    ///     . . . . . . . . . .
    ///     . . . . . . . . . .     $ = Empress
    ///     . . . . . . . . . .     ^ = Princess
    ///     . . . . . . . . . .
    ///     p p p p p p p p p p
    ///     R N B $ Q K ^ B N R
    /// </summary>
    public class Capablanca : Chess {
        private new const int BOARD_WIDTH = 10;
        private new const int BOARD_HEIGHT = 8;

        public Capablanca() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            PawnPromotionOptions = new Piece[6]{ Piece.Queen, Piece.Empress, Piece.Princess, Piece.Rook, Piece.Bishop, Piece.Knight };
            castlingDistance = 3;
        }

        public override string ToString() {
            return "Capablanca's Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(5, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(5, BLACK_BACKROW)));

            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(9, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(9, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            AddPieceToBoard(new Empress(Team.WHITE, new BoardCoord(3, WHITE_BACKROW)));
            AddPieceToBoard(new Empress(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            AddPieceToBoard(new Princess(Team.WHITE, new BoardCoord(6, WHITE_BACKROW)));
            AddPieceToBoard(new Princess(Team.BLACK, new BoardCoord(6, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            GetPiecesOfType<Princess>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });
            GetPiecesOfType<Empress>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });

            return possibleCheckThreats;
        }

        protected override bool TryPerformCastlingRookMoves(ChessPiece mover, ref string moveNotation) {
            if (mover.MoveCount == 1 && mover == currentRoyalPiece) {

                if (mover.GetBoardPosition().x == 2) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(0, mover.GetBoardPosition().y)).occupier;
                    MakeDirectMove(castlingPiece, new BoardCoord(3, mover.GetBoardPosition().y), false);
                    moveNotation = "O-O-O";
                    return true;

                } else if (mover.GetBoardPosition().x == 8) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(BOARD_WIDTH - 1, mover.GetBoardPosition().y)).occupier;
                    MakeDirectMove(castlingPiece, new BoardCoord(7, mover.GetBoardPosition().y), false);
                    moveNotation = "O-O";
                    return true;

                }
            }
            return false;
        }
    }
}