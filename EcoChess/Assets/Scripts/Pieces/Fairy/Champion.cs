using UnityEngine;
using System.Collections.Generic;

public class Champion : ChessPiece {
    public Champion(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Champion;
    }
    public Champion(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Champion;
    }

    public override string ToString() {
        return GetTeam() + "_Champion";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 0, 1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 0, -1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 1, 0, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -1, 0, cap: 1));

        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 0, 2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 0, -2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 2, 0, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -2, 0, cap: 1));

        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 2, 2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -2, 2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 2, -2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -2, -2, cap: 1));

        return moves;
    }
}
