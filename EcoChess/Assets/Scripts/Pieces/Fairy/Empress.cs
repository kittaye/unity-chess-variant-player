using UnityEngine;
using System.Collections.Generic;

public class Empress : ChessPiece {
    public Empress(Team team, BoardCoord position) : base(team, position) {

    }

    public override string ToString() {
        return GetTeam() + "_Empress";
    }

    public override void CalculateTemplateMoves() {
        // Rook movements
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.Up));
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.Left));
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.Down));
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.Right));

        // Vertical "L" movements
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, 1, 2, cap: 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, -1, 2, cap: 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, 1, -2, cap: 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, -1, -2, cap: 1));

        // Horizontal "L" movements
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, 2, 1, cap: 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, -2, 1, cap: 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, 2, -1, cap: 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, -2, -1, cap: 1));
    }
}
