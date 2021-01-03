
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     R N B Q K B N R
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     r n b q k b n r
    /// </summary>
    public class UpsideDown : Chess {
        public UpsideDown() : base() { }

        public override string ToString() {
            return "Upside-Down Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by G. P. Jelliss (1991)",
                this.ToString() + " is a variant that swaps the teams' positions with each other (making pawns one move away from promoting)",
                "Checkmate.",
                VariantHelpDetails.rule_None,
                "https://www.chessvariants.com/diffsetup.dir/upside.html"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(3, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(3, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(7, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(7, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, BLACK_PAWNROW));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }
    }
}