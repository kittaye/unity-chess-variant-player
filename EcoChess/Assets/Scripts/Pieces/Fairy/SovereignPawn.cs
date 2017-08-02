using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sovereign Pawns are specific only to the Sovereign Chess variant and should not be used elsewhere.
/// </summary>
public class SovereignPawn : Pawn {
    public enum Quadrant { BottomLeft, BottomRight, UpLeft, UpRight}
    public Quadrant pieceQuadrant;

    public SovereignPawn(Team team, BoardCoord position, Quadrant quadrant) : base(team, position) {
        this.pieceQuadrant = quadrant;
    }
    public SovereignPawn(Team team, string algebraicKeyPosition, Quadrant quadrant) : base(team, algebraicKeyPosition) {
        this.pieceQuadrant = quadrant;
    }

    public override string ToString() {
        return GetTeam() + "_Pawn";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        BoardCoord pos = GetBoardPosition();
        uint moveCap = (pos.x <= 1 || pos.x >= 14 || pos.y <= 1 || pos.y >= 14) ? initialMoveLimit : 1;

        switch (pieceQuadrant) {
            case Quadrant.BottomLeft:
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Up, cap: moveCap, threatAttackLimit: 0, teamSensitive: false));
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Right, cap: moveCap, threatAttackLimit: 0, teamSensitive: false));

                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight, cap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft, cap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownRight, cap: 1, threatsOnly: true, teamSensitive: false));
                break;
            case Quadrant.BottomRight:
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Up, cap: moveCap, threatAttackLimit: 0, teamSensitive: false));
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Left, cap: moveCap, threatAttackLimit: 0, teamSensitive: false));

                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight, cap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft, cap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownLeft, cap: 1, threatsOnly: true, teamSensitive: false));
                break;
            case Quadrant.UpLeft:
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Down, cap: moveCap, threatAttackLimit: 0, teamSensitive: false));
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Right, cap: moveCap, threatAttackLimit: 0, teamSensitive: false));

                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight, cap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownLeft, cap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownRight, cap: 1, threatsOnly: true, teamSensitive: false));
                break;
            case Quadrant.UpRight:
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Down, cap: moveCap, threatAttackLimit: 0, teamSensitive: false));
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Left, cap: moveCap, threatAttackLimit: 0, teamSensitive: false));

                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft, cap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownLeft, cap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownRight, cap: 1, threatsOnly: true, teamSensitive: false));
                break;
            default:
                break;
        }
        return moves;
    }
}
