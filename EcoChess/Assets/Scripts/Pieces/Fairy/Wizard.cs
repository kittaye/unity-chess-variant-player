using System.Collections.Generic;

public class Wizard : ChessPiece {
    public Wizard(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.Wizard;
    }
    public Wizard(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.Wizard;
    }

    public override string GetCanonicalName() {
        return "Wizard";
    }

    public override string GetLetterNotation() {
        return "W";
    }

    protected override void InitSpecificMoveSet() {
        m_SpecificMoveSet = new BoardCoord[12] {
            // Long Vertical "L" movements
            new BoardCoord(1, 3),
            new BoardCoord(-1, 3),
            new BoardCoord(1, -3),
            new BoardCoord(-1, -3),

            // Long Horizontal "L" movements
            new BoardCoord(3, 1),
            new BoardCoord(-3, 1),
            new BoardCoord(3, -1),
            new BoardCoord(-3, -1),

            // Diagonal movements
            new BoardCoord(1, 1),
            new BoardCoord(-1, 1),
            new BoardCoord(1, -1),
            new BoardCoord(-1, -1)
        };
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>(12);

        moves.AddRange(TryGetTemplateMovesFromSpecificMoveSet());

        return moves;
    }
}
