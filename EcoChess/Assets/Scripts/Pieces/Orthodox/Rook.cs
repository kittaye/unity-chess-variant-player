using UnityEngine;
using System.Collections.Generic;

public class Rook : ChessPiece {
    public Rook(Team team, BoardCoord position) : base(team, position) {

    }

    public override string ToString() {
        return GetTeam() + "_Rook";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Up));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Left));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Down));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Right));

        return moves;
    }
}
