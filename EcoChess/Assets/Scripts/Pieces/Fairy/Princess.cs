using System.Collections.Generic;

public class Princess : ChessPiece {
    public Princess(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.Princess;
    }
    public Princess(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.Princess;
    }

    public override string GetCanonicalName() {
        return "Princess";
    }

    public override string GetLetterNotation() {
        return "P";
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

        // Bishop movements
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpRight));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpLeft));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownRight));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownLeft));

        // Knight movements
        moves.AddRange(TryGetTemplateMovesFromSpecificMoveSet());

        return moves;
    }
}
