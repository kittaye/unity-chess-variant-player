using System.Collections.Generic;

public class Nightrider : ChessPiece {
    public Nightrider(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.Nightrider;
    }
    public Nightrider(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.Nightrider;
    }

    public override string GetCanonicalName() {
        return "Nightrider";
    }

    public override string GetLetterNotation() {
        return "NR";
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

        // Infinitely extended knight movements
        for (int i = 0; i < m_SpecificMoveSet.Length; i++) {
            moves.AddRange(TryGetDirectionalTemplateMoves(m_SpecificMoveSet[i]));
        }

        return moves;
    }
}
