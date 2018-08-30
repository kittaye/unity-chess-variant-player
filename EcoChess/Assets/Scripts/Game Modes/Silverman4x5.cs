using System.Collections;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Silverman4x5.cs is a chess variant with a smaller board layout.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Piece rules: No castling, no pawn double moves, pawn promotes to queen or rook only.
    /// Board layout:
    ///     r q k r
    ///     p p p p
    ///     . . . .
    ///     p p p p
    ///     R Q K R
    /// </summary>
    public class Silverman4x5 : Chess {
        private new const uint BOARD_WIDTH = 4;
        private new const uint BOARD_HEIGHT = 5;

        public Silverman4x5() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            PawnPromotionOptions = new Piece[] { Piece.Queen, Piece.Rook };
            AllowCastling = false;
            AllowEnpassantCapture = false;
        }

        public override string ToString() {
            return "Silverman 4x5";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(2, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(2, BLACK_BACKROW)));

            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(3, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(1, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(1, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW), initialMoveLimit: 1));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW), initialMoveLimit: 1));
            }
        }
    }
}
