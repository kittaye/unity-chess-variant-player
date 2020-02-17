using System.Collections.Generic;

public class Queen : ChessPiece {
    public Queen(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Queen;
    }
    public Queen(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Queen;
    }
    public Queen(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Queen;
    }
    public Queen(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
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

        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.UpRight));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.UpLeft));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.DownLeft));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.DownRight));

        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.Up));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.Left));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.Down));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.Right));

        return moves;
    }
}
