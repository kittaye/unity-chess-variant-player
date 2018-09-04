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

    public override string ToString() {
        return GetTeam() + "_Queen";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();
        for (int i = 0; i <= 7; i++) {
            moves.AddRange(chessGame.TryGetDirectionalMoves(this, (MoveDirection)i));
        }
        return moves;
    }

    public override string GetLetterNotation() {
        return "Q";
    }
}
