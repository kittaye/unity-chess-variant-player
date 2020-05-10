using System.Collections.Generic;

public class Rook : ChessPiece {
    public Rook(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.Rook;
    }
    public Rook(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.Rook;
    }

    public override string GetCanonicalName() {
        return "Rook";
    }

    public override string GetLetterNotation() {
        return "R";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Left));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Down));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Right));

        return moves;
    }
}
