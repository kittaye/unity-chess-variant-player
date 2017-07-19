using UnityEngine;
using System.Collections.Generic;

public class BerolinaPawn : Pawn {
    public BerolinaPawn(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.BerolinaPawn;
    }
    public BerolinaPawn(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.BerolinaPawn;
    }

    public override string ToString() {
        return GetTeam() + "_BerolinaPawn";
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
