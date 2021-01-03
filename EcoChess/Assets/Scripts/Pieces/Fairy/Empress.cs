using System.Collections.Generic;

public class Empress : ChessPiece {
    public Empress(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.Empress;
    }
    public Empress(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.Empress;
    }

    public override string GetCanonicalName() {
        return "Empress";
    }

    public override string GetLetterNotation() {
        return "E";
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

        // Rook movements
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Left));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Down));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Right));

        // Knight movements
        moves.AddRange(TryGetTemplateMovesFromSpecificMoveSet());

        return moves;
    }
}
