using System.Collections.Generic;

public class King : ChessPiece {
    public King(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.King;
    }
    public King(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.King;
    }
    public King(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.King;
    }
    public King(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.King;
    }

    public override string ToString() {
        return GetTeam() + "_King";
    }

    public override string GetLetterNotation() {
        return "K";
    }

    protected override void InitSpecificMoveSet() {
        m_SpecificMoveSet = new BoardCoord[8] {
            // Right, left, up, down
            new BoardCoord(0, 1),
            new BoardCoord(0, -1),
            new BoardCoord(1, 0),
            new BoardCoord(-1, 0),

            // Diagonals
            new BoardCoord(1, 1),
            new BoardCoord(1, -1),
            new BoardCoord(-1, 1),
            new BoardCoord(-1, -1)
        };
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(TryGetTemplateMovesFromSpecificMoveSet());

        return moves;
    }
}
