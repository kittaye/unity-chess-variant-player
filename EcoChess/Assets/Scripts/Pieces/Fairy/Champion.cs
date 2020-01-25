using UnityEngine;
using System.Collections.Generic;

public class Champion : ChessPiece {
    public Champion(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Champion;
    }
    public Champion(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Champion;
    }
    public Champion(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Champion;
    }
    public Champion(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Champion;
    }

    public override string ToString() {
        return GetTeam() + "_Champion";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 0, 1, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 0, -1, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 1, 0, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -1, 0, moveCap: 1));

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 0, 2, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 0, -2, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 2, 0, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -2, 0, moveCap: 1));

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 2, 2, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -2, 2, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 2, -2, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -2, -2, moveCap: 1));

        return moves;
    }

    public override string GetLetterNotation() {
        return "C";
    }
}
