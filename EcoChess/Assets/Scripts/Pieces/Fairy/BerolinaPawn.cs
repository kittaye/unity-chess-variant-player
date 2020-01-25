using UnityEngine;
using System.Collections.Generic;

public class BerolinaPawn : Pawn {
    public BerolinaPawn(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.BerolinaPawn;
    }
    public BerolinaPawn(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.BerolinaPawn;
    }
    public BerolinaPawn(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.BerolinaPawn;
    }
    public BerolinaPawn(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.BerolinaPawn;
    }

    public override string ToString() {
        return GetTeam() + "_BerolinaPawn";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        uint moveCap = (MoveCount == 0) ? initialMoveLimit : 1;

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight, moveCap: moveCap, threatAttackLimit: 0));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft, moveCap: moveCap, threatAttackLimit: 0));

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Up, moveCap: 1, threatsOnly: true));

        return moves;
    }

    public override string GetLetterNotation() {
        return "BP";
    }
}
