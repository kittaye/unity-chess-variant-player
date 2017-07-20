using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sovereign Pawns are specific only to the Sovereign Chess variant and should not be used elsewhere.
/// </summary>
public class SovereignPawn : Pawn {
    public SovereignPawn(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.SovereignPawn;
    }
    public SovereignPawn(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.SovereignPawn;
    }

    public override string ToString() {
        return "WHITE_Pawn";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();
        if (MoveCount == 0) {
            moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight, cap: initialMoveLimit, threatAttackLimit: 0));
            moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft, cap: initialMoveLimit, threatAttackLimit: 0));
        } else {
            moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight, cap: 1, threatAttackLimit: 0));
            moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft, cap: 1, threatAttackLimit: 0));
        }

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Up, cap: 1, threatsOnly: true));

        return moves;
    }
}
