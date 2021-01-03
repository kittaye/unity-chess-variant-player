using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: FIDE standard but queens are amazons.
    /// </summary>
    public class AmazonChess : Chess {
        public AmazonChess() : base() {
            SelectedPawnPromotion = Piece.Amazon;
            PawnPromotionOptions = new Piece[4] { Piece.Amazon, Piece.Rook, Piece.Bishop, Piece.Knight };
        }

        public override string ToString() {
            return "Amazon Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented in the 1900s",
                this.ToString() + " is a variant that replaces queens with amazons.",
                "Checkmate.",
                "- Pawns may not promote to a queen, but may promote to an amazon.",
                "http://www.pathguy.com/chess/AmazonCh.html"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(7, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(7, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Amazon, Team.WHITE, new BoardCoord(3, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Amazon, Team.BLACK, new BoardCoord(3, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
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
