using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Legan Pawns are specific to the Legan Chess variant, but could be used elsewhere (however no factory support).
/// </summary>
public class LeganPawn : Pawn {

    public LeganPawn(Team team, BoardCoord position) : base(team, position, false, 1) { }
    public LeganPawn(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition, false, 1) { }
    public LeganPawn(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
    }
    public LeganPawn(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
        : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
    }

    public override string GetCanonicalName() {
        return "Pawn";
    }

    public override string GetLetterNotation() {
        return "LP";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.UpLeft, moveCap: 1, threatAttackLimit: 0));

        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.Up, moveCap: 1, threatsOnly: true));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.Left, moveCap: 1, threatsOnly: true));

        return moves;
    }
}
