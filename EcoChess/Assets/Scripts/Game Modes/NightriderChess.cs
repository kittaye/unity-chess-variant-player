using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// NightriderChess.cs is a chess variant with Nightriders replacing one of the knights.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox + Nightriders.
    /// Board layout:
    ///     r # b q k b n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .     # = Nightrider
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     R # B Q K B N R
    /// </summary>
    public class NightriderChess : Chess {
        public NightriderChess() : base() {
            PawnPromotionOptions = new Piece[5] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight, Piece.Nightrider };
        }

        public override string ToString() {
            return "Nightrider Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(7, WHITE_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(3, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            AddPieceToBoard(new Nightrider(Team.WHITE, new BoardCoord(1, WHITE_BACKROW)));
            AddPieceToBoard(new Nightrider(Team.BLACK, new BoardCoord(1, BLACK_BACKROW)));
            AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(6, WHITE_BACKROW)));
            AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(6, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> checkThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            GetPiecesOfType<Nightrider>(pieceToCheck.GetOpposingTeam()).ForEach((x) => { checkThreats.Add(x); });

            return checkThreats;
        }
    }
}
