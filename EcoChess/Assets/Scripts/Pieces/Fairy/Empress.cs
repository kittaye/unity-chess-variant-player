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

    public override string ToString() {
        return GetTeam() + "_Empress";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();
        // Rook movements
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Up));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Left));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Down));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Right));

        // Vertical "L" movements
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 1, 2, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -1, 2, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 1, -2, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -1, -2, moveCap: 1));

        // Horizontal "L" movements
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 2, 1, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -2, 1, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 2, -1, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -2, -1, moveCap: 1));

        return moves;
    }

    public override string GetLetterNotation() {
        return "E";
    }
}
