using System.Collections.Generic;

public class Knight : ChessPiece {
    public Knight(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.Knight;
    }
    public Knight(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.Knight;
    }

    public override string GetCanonicalName() {
        return "Knight";
    }

    public override string GetLetterNotation() {
        return "N";
    }

    protected override void InitSpecificMoveSet() {
        m_SpecificMoveSet = new BoardCoord[8] {
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
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(TryGetTemplateMovesFromSpecificMoveSet());
        
        return moves;
    }
}
