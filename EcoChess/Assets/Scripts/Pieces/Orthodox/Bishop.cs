using System.Collections.Generic;

public class Bishop : ChessPiece {
    public Bishop(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Bishop;
    }
    public Bishop(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Bishop;
    }
    public Bishop(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Bishop;
    }
    public Bishop(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Bishop;
    }

    public override string ToString() {
        return GetTeam() +"_Bishop";
    }

    public override string GetLetterNotation() {
        return "B";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.UpRight));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.UpLeft));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.DownLeft));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.DownRight));

        return moves;
    }
}
