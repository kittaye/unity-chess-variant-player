using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///             K
    ///           Q p
    ///         B p .
    ///       N p . .
    ///     R p . . . 
    ///     p . . . .
    ///     . . . . p
    ///     . . . p R
    ///     . . p N 
    ///     . p B
    ///     p Q   
    ///     K 
    /// </summary>
    public class LadderChess : Chess {
        private new const int BOARD_WIDTH = 5;
        private new const int BOARD_HEIGHT = 12;
        private List<BoardCoord> promotionSquares;

        public LadderChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            Board.RemoveBoardCoordinates(new string[]
            { "b1", "c1", "d1", "e1",
              "c2", "d2", "e2",
              "d3", "e3",
              "e4",
              "a12", "b12","c12","d12",
              "a11", "b11","c11",
              "a10","b10",
              "a9"
            });

            promotionSquares = new List<BoardCoord>(10);
            AddPromotionSquare("a1");
            AddPromotionSquare("b2");
            AddPromotionSquare("c3");
            AddPromotionSquare("d4");
            AddPromotionSquare("e5");
            AddPromotionSquare("a8");
            AddPromotionSquare("b9");
            AddPromotionSquare("c10");
            AddPromotionSquare("d11");
            AddPromotionSquare("e12");

            AllowCastling = false;
        }

        public override string ToString() {
            return "Ladder Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Sergey Sirotkin (2000)",
                this.ToString() + " is a variant on an irregular board shape (5x12).",
                "Checkmate.",
                VariantHelpDetails.rule_NoCastling + "\n" +
                "- Note: Pawns promote as usual at the ends of each file.",
                "https://www.chessvariants.com/40.dir/ladderchess.html"
            );
        }

        private void AddPromotionSquare(string algebraicKeyPosition) {
            BoardCoord coord;
            if (Board.TryGetCoordWithKey(algebraicKeyPosition, out coord)) {
                promotionSquares.Add(coord);
            }
        }

        protected override bool IsAPromotionMove(BoardCoord move) {
            return promotionSquares.Contains(move);
        }

        protected override bool PerformedAPromotionMove(Pawn mover) {
            return promotionSquares.Contains(mover.GetBoardPosition());
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, "a1"));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, "e12"));

            AddPromotionSquare("a1");
            AddPromotionSquare("b2");
            AddPromotionSquare("c3");
            AddPromotionSquare("d4");
            AddPromotionSquare("e5");
            AddPromotionSquare("a8");
            AddPromotionSquare("b9");
            AddPromotionSquare("c10");
            AddPromotionSquare("d11");
            AddPromotionSquare("e12");

            AddPieceToBoard(new Queen(Team.WHITE, "b2"));
            AddPieceToBoard(new Queen(Team.BLACK, "d11"));

            AddPieceToBoard(new Bishop(Team.WHITE, "c3"));
            AddPieceToBoard(new Bishop(Team.BLACK, "c10"));

            AddPieceToBoard(new Knight(Team.WHITE, "d4"));
            AddPieceToBoard(new Knight(Team.BLACK, "b9"));

            AddPieceToBoard(new Rook(Team.WHITE, "e5"));
            AddPieceToBoard(new Rook(Team.BLACK, "a8"));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, x + 1)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, x + 6)));
            }
        }
    }
}