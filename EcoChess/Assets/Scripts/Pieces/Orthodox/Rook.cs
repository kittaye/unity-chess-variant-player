using UnityEngine;
using System.Collections.Generic;

public class Rook : ChessPiece {
    public Rook(Team team, BoardCoord position) : base(team, position) {

    }

    public override string ToString() {
        return GetTeam() + "_Rook";
    }

    public override void CalculateTemplateMoves() {
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.Up));
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.Left));
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.Down));
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.Right));
    }
}
