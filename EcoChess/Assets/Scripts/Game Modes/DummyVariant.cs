
namespace ChessGameModes {
    public class DummyVariant : Chess {

        public DummyVariant() : base() {
        }

        public override string ToString() {
            return "Dummy Variant";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, 2));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW - 2));

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(3, 3));
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(4, 3));
        }
    }
}
