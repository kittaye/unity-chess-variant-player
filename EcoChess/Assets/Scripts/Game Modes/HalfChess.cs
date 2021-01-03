
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r n . . . . N R
    ///     q b . . . . B Q
    ///     k b . . . . B K
    ///     r n . . . . N R
    /// </summary>
    public class HalfChess : Chess {
        private new const int BOARD_HEIGHT = 4;

        public HalfChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            AllowCastling = false;
            AllowEnpassantCapture = false;
        }

        public override string ToString() {
            return "Half Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by John Groeneman (1960s)",
                this.ToString() + " is a variant on a 4x8 board without pawns.",
                "Checkmate.",
                VariantHelpDetails.rule_NoCastling,
                "https://www.chessvariants.com/small.dir/halfchess.html"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, "h2");
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, "a2");

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, "h3");
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, "a3");

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, "h1");
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, "h4");
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, "a1");
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, "a4");
        
            AddNewPieceToBoard(Piece.Knight, Team.WHITE, "g1");
            AddNewPieceToBoard(Piece.Knight, Team.WHITE, "g4");
            AddNewPieceToBoard(Piece.Knight, Team.BLACK, "b1");
            AddNewPieceToBoard(Piece.Knight, Team.BLACK, "b4");

            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, "g2");
            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, "g3");
            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, "b2");
            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, "b3");
        }
    }
}