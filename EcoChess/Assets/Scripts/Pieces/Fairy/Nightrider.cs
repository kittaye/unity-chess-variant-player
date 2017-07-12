using UnityEngine;
using System.Collections.Generic;

public class Nightrider : ChessPiece {
    public Nightrider(Team team, BoardCoord position) : base(team, position) {
    }

    public override string ToString() {
        return GetTeam() + "_Nightrider";
    }

    public override void CalculateTemplateMoves() {
        // Vertical "L" movements
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, 1, 2));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, -1, 2));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, 1, -2));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, -1, -2));

        // Horizontal "L" movements
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, 2, 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, -2, 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, 2, -1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, -2, -1));
    }
}
