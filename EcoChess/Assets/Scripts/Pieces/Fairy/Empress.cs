using UnityEngine;
using System.Collections.Generic;

public class Empress : ChessPiece {
    public Empress(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Empress;
    }
    public Empress(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Empress;
    }
    public Empress(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Empress;
    }
    public Empress(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
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
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.Up));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.Left));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.Down));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.Right));

        // Knight movements
        moves.AddRange(TryGetTemplateMovesFromSpecificMoveSet());

        return moves;
    }
}
