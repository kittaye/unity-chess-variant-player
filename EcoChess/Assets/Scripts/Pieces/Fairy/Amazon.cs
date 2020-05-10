using System.Collections.Generic;

public class Amazon : ChessPiece {
    public Amazon(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.Amazon;
    }
    public Amazon(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.Amazon;
    }

    public override string GetCanonicalName() {
        return "Amazon";
    }

    public override string GetLetterNotation() {
        return "A";
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

        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpRight));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpLeft));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownLeft));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownRight));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Left));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Down));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Right));

        moves.AddRange(TryGetTemplateMovesFromSpecificMoveSet());

        return moves;
    }
}
