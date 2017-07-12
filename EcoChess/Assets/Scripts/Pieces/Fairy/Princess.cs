using UnityEngine;
using System.Collections.Generic;

public class Princess : ChessPiece {
    public Princess(Team team, BoardCoord position) : base(team, position) {

    }

    public override string ToString() {
        return GetTeam() + "_Princess";
    }

    public override void CalculateTemplateMoves() {
        // Bishop movements
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight));
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft));
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownRight));
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownLeft));

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
