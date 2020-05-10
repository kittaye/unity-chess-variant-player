
public enum Piece { King, Queen, Knight, Bishop, Rook, Pawn, Empress, Princess, Grasshopper, Nightrider, BerolinaPawn, LeganPawn, SovereignPawn, Wizard, Champion, Amazon }

public static class ChessPieceFactory {
    public static ChessPiece Create(Piece piece, Team team, BoardCoord position, Board board) {
        switch (piece) {
            case Piece.King:
                return new King(team, position, board);
            case Piece.Queen:
                return new Queen(team, position, board);
            case Piece.Knight:
                return new Knight(team, position, board);
            case Piece.Bishop:
                return new Bishop(team, position, board);
            case Piece.Rook:
                return new Rook(team, position, board);
            case Piece.Pawn:
                return new Pawn(team, position, board);

            case Piece.Empress:
                return new Empress(team, position, board);
            case Piece.Princess:
                return new Princess(team, position, board);
            case Piece.Grasshopper:
                return new Grasshopper(team, position, board);
            case Piece.Nightrider:
                return new Nightrider(team, position, board);
            case Piece.BerolinaPawn:
                return new BerolinaPawn(team, position, board);
            case Piece.LeganPawn:
                return new LeganPawn(team, position, board);
            case Piece.SovereignPawn:
                return new SovereignPawn(team, position, board);
            case Piece.Wizard:
                return new Wizard(team, position, board);
            case Piece.Champion:
                return new Champion(team, position, board);
            case Piece.Amazon:
                return new Amazon(team, position, board);
            default:
                return null;
        }
    }
}
