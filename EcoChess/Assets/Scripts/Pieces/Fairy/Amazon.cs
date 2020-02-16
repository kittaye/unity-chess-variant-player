using System.Collections.Generic;

public class Amazon : ChessPiece {
    public Amazon(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Amazon;
    }
    public Amazon(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Amazon;
    }
    public Amazon(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Amazon;
    }
    public Amazon(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Amazon;
    }

    public override string ToString() {
        return GetTeam() + "_Amazon";
    }

    public override string GetLetterNotation() {
        return "A";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        for (int i = 0; i <= 7; i++) {
            moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, (MoveDirection)i));
        }

        moves.AddRange(Knight.moveset);

        return moves;
    }
}
