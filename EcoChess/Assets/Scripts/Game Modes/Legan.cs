using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: 
    ///     k n b r p . . .
    ///     b q p p . . . .
    ///     n p p . . . . .
    ///     r p . p . . . p
    ///     p . . . p . p R
    ///     . . . . . p p N
    ///     . . . . p p Q B
    ///     . . . p R B N K
    /// </summary>
    public class Legan : Chess {
        private List<BoardCoord> promotionSquares;

        public Legan() : base() {
            promotionSquares = new List<BoardCoord>(14);
            AddPromotionSquare("a8");
            AddPromotionSquare("b8");
            AddPromotionSquare("c8");
            AddPromotionSquare("d8");
            AddPromotionSquare("a7");
            AddPromotionSquare("a6");
            AddPromotionSquare("a5");

            AddPromotionSquare("e1");
            AddPromotionSquare("f1");
            AddPromotionSquare("g1");
            AddPromotionSquare("h1");
            AddPromotionSquare("h2");
            AddPromotionSquare("h3");
            AddPromotionSquare("h4");

            AllowCastling = false;
            AllowEnpassantCapture = false;
        }

        public override string ToString() {
            return "Legan Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by L. Legan (1913)",
                this.ToString() + " is a variant that involves a custom board layout with unique pawn rules.",
                "Checkmate.",
                "- Pawn's move and attack behaviours are switched.\n" +
                "- Pawns promote at the opposing team's corner of the board. Each square a major piece starts on is a promotion square.\n" +
                VariantHelpDetails.rule_NoCastling + "\n" +
                VariantHelpDetails.rule_NoEnpassantCapture,
                "https://en.wikipedia.org/wiki/Legan_chess"
            );
        }

        private void AddPromotionSquare(string algebraicKeyPosition) {
            BoardCoord coord;
            if (Board.TryGetCoordWithKey(algebraicKeyPosition, out coord)) {
                promotionSquares.Add(coord);
            }
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, "h1");
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, "a8");

            AddNewPieceToBoard(Piece.Knight, Team.WHITE, "g1");
            AddNewPieceToBoard(Piece.Knight, Team.WHITE, "h3");
            AddNewPieceToBoard(Piece.Knight, Team.BLACK, "b8");
            AddNewPieceToBoard(Piece.Knight, Team.BLACK, "a6");

            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, "h2");
            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, "f1");
            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, "c8");
            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, "a7");

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, "e1");
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, "h4");
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, "a5");
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, "d8");

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, "g2");
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, "b7");

            // White pawns
            AddNewPieceToBoard(Piece.LeganPawn, Team.WHITE, "f2");
            AddNewPieceToBoard(Piece.LeganPawn, Team.WHITE, "g3");
            AddNewPieceToBoard(Piece.LeganPawn, Team.WHITE, "e4");
            BoardCoord coord = new BoardCoord(3, 0);
            while (Board.ContainsCoord(coord)) {
                AddNewPieceToBoard(Piece.LeganPawn, Team.WHITE, coord);
                coord.x++;
                coord.y++;
            }

            // Black pawns
            AddNewPieceToBoard(Piece.LeganPawn, Team.BLACK, "b6");
            AddNewPieceToBoard(Piece.LeganPawn, Team.BLACK, "c7");
            AddNewPieceToBoard(Piece.LeganPawn, Team.BLACK, "d5");
            coord = new BoardCoord(0, 3);
            while (Board.ContainsCoord(coord)) {
                AddNewPieceToBoard(Piece.LeganPawn, Team.BLACK, coord);
                coord.x++;
                coord.y++;
            }
        }

        protected override bool IsAPromotionMove(BoardCoord move) {
            return promotionSquares.Contains(move);
        }

        protected override bool PerformedAPromotionMove(Pawn mover) {
            return promotionSquares.Contains(mover.GetBoardPosition());
        }
    }
}