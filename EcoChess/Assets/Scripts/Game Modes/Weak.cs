using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Weak.cs is a chess variant with a custom initial board layout.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Piece rules: Black may only promote to a knight.
    /// Board layout:
    ///     n n n n k n n n
    ///     p p p p p p p p
    ///     . . p . . p . .
    ///     . p p p p p p .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     R N B Q K B N R
    /// </summary>
    public class Weak : Chess {
        private Piece[] whitePromotionOptions;
        private Piece[] blackPromotionOptions;

        public Weak() : base() {
            whitePromotionOptions = PawnPromotionOptions;
            blackPromotionOptions = new Piece[0];
        }

        public override string ToString() {
            return "Weak!";
        }

        public override void OnTurnComplete() {
            base.OnTurnComplete();
            if(GetCurrentTeamTurn() == Team.WHITE) {
                SelectedPawnPromotion = Piece.Queen;
                PawnPromotionOptions = whitePromotionOptions;
            } else {
                SelectedPawnPromotion = Piece.Knight;
                PawnPromotionOptions = blackPromotionOptions;
            }
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, "d1"));
            AddPieceToBoard(new Bishop(Team.WHITE, "c1"));
            AddPieceToBoard(new Bishop(Team.WHITE, "f1"));
            AddPieceToBoard(new Knight(Team.WHITE, "b1"));
            AddPieceToBoard(new Knight(Team.WHITE, "g1"));
            AddPieceToBoard(new Rook(Team.WHITE, "a1"));
            AddPieceToBoard(new Rook(Team.WHITE, "h1"));

            AddPieceToBoard(new Pawn(Team.BLACK, "c6", initialMoveLimit: 1));
            AddPieceToBoard(new Pawn(Team.BLACK, "f6", initialMoveLimit: 1));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if(x != 4) {
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
                if(x >= 1 && x <= BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW - 2), initialMoveLimit: 1));
                }
            }
        }
    }
}
