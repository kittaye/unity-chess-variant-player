using System.Collections.Generic;

public class Champion : ChessPiece {
    public Champion(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.Champion;
    }
    public Champion(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.Champion;
    }

    public override string GetCanonicalName() {
        return "Champion";
    }

    public override string GetLetterNotation() {
        return "C";
    }

    protected override void InitSpecificMoveSet() {
        m_SpecificMoveSet = new BoardCoord[12] {
            new BoardCoord(0, 1),
            new BoardCoord(0, -1),
            new BoardCoord(1, 0),
            new BoardCoord(-1, 0),

            new BoardCoord(0, 2),
            new BoardCoord(0, -2),
            new BoardCoord(2, 0),
            new BoardCoord(-2, 0),

            new BoardCoord(2, 2),
            new BoardCoord(-2, 2),
            new BoardCoord(2, -2),
            new BoardCoord(-2, -2)
        };
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>(12);

        moves.AddRange(TryGetTemplateMovesFromSpecificMoveSet());

        return moves;
    }
}
