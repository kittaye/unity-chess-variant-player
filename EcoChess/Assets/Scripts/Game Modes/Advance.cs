
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r n b q k b n r
    ///     . . . . . . . . 
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p 
    ///     . . . . . . . .
    ///     R N B Q K B N R
    /// </summary>
    public class Advance : Chess {
        public Advance() : base() {
        }

        public override string ToString() {
            return "Advanced Pawn Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Tony Paletta (1980)",
                this.ToString() + " is a variant where the pawns are positioned a further rank forward.",
                "Checkmate.",
                VariantHelpDetails.rule_NoPawnDoubleMove,
                "https://greenchess.net/rules.php?v=advanced-pawn"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(7, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(7, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(3, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(3, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                ((Pawn)AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW + 1))).initialMoveLimit = 1;
                ((Pawn)AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW - 1))).initialMoveLimit = 1;

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }
    }
}
