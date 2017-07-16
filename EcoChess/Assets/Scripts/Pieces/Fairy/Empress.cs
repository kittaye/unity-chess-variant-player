using UnityEngine;
using System.Collections.Generic;

public class Empress : ChessPiece {
    public Empress(Team team, BoardCoord position) : base(team, position) {
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
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 1, 2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -1, 2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 1, -2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -1, -2, cap: 1));

        // Horizontal "L" movements
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 2, 1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -2, 1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 2, -1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -2, -1, cap: 1));

        return moves;
    }
}
