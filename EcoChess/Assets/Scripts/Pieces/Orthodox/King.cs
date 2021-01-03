using System.Collections.Generic;

public class King : ChessPiece {
    public King(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.King;
    }
    public King(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.King;
    }

    public override string GetCanonicalName() {
        return "King";
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
