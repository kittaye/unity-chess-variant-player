using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: 
    ///     r n b q k b n r 
    ///     p p p p p p p p 
    ///     . . . . . . . . 
    ///     . . . . . . . . 
    ///     . . . . . . . .     A = Amazon
    ///     . . . . . . . . 
    ///     . . . . . . . . 
    ///     . . . . A . . . 
    /// </summary>
    public class Maharajah : Chess {

        public Maharajah() : base() {
            AllowPawnPromotion = false;
        }

        public override string ToString() {
            return "Maharajah Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented in India (1800s)",
                this.ToString() + " is a variant that pits the black FIDE army against one amazon.",
                "Checkmate.",
                VariantHelpDetails.rule_NoPawnPromotion + "\n" +
                "- Note: The white amazon is royal and must be checkmated for black to win.",
                "https://en.wikipedia.org/wiki/Maharajah_and_the_Sepoys"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = AddNewPieceToBoard(Piece.Amazon, Team.WHITE, "e1");
            opposingRoyalPiece = AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(7, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(3, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Amazon>(pieceToCheck.GetOpposingTeam()));

            return possibleCheckThreats;
        }
    }
}