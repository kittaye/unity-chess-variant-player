using System.Collections.Generic;

public class Queen : ChessPiece {
    public Queen(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.Queen;
    }
    public Queen(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.Queen;
    }

    public override string GetCanonicalName() {
        return "Queen";
    }

    public override string GetLetterNotation() {
        return "Q";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpRight));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpLeft));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownLeft));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownRight));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Left));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Down));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Right));

        return moves;
    }
}
