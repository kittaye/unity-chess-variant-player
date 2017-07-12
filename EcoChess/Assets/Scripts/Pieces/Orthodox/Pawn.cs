using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece {
    public readonly List<Pawn> enPassantTargets;
    public readonly uint initialMoveLimit;
    public readonly bool canEnPassantCapture;
    public bool validEnPassant;

    public Pawn(Team team, BoardCoord position, bool canEnPassantCapture = true, uint initialMoveLimit = 2) 
        : base(team, position) {
        this.enPassantTargets = new List<Pawn>(2);
        this.validEnPassant = false;
        this.canEnPassantCapture = canEnPassantCapture;
        this.initialMoveLimit = initialMoveLimit;
    }

    public override string ToString() {
        return GetTeam() + "_Pawn";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();
        if (MoveCount == 0) {
            moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Up, cap: initialMoveLimit, threatAttackLimit: 0));
        } else {
            moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Up, cap: 1, threatAttackLimit: 0));
        }

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft, cap: 1, threatsOnly: true));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight, cap: 1, threatsOnly: true));

        return moves;
    }
}
