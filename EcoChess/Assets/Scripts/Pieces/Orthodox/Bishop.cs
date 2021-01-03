using System.Collections.Generic;

public class Bishop : ChessPiece {
    public Bishop(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.Bishop;
    }
    public Bishop(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.Bishop;
    }

    public override string GetCanonicalName() {
        return "Bishop";
    }

    public override string GetLetterNotation() {
        return "B";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpRight));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpLeft));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownLeft));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownRight));

        return moves;
    }
}
