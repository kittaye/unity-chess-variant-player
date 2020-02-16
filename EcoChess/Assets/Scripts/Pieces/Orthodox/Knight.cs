using System.Collections.Generic;

public class Knight : ChessPiece {
    public Knight(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Knight;
    }
    public Knight(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Knight;
    }
    public Knight(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping)
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Knight;
    }
    public Knight(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Knight;
    }

    public override string ToString() {
        return GetTeam() + "_Knight";
    }

    public override string GetLetterNotation() {
        return "N";
    }

    public static BoardCoord[] moveset = new BoardCoord[8] {
            // Vertical "L" movements
            new BoardCoord(1, 2),
            new BoardCoord(-1, 2),
            new BoardCoord(1, -2),
            new BoardCoord(-1, -2),

            // Horizontal "L" movements
            new BoardCoord(2, 1),
            new BoardCoord(-2, 1),
            new BoardCoord(2, -1),
            new BoardCoord(-2, -1)
    };

    protected override void InitSpecificMoveSet() {
        m_SpecificMoveSet = moveset;
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(TryGetTemplateMovesFromSpecificMoveSet());
        
        return moves;
    }
}
