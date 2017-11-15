using UnityEngine;
using System.Collections.Generic;

public class Princess : ChessPiece {
    public Princess(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Princess;
    }
    public Princess(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Princess;
    }
    public Princess(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Princess;
    }
    public Princess(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Princess;
    }

    public override string ToString() {
        return GetTeam() + "_Princess";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();
        // Bishop movements
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownRight));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownLeft));

        // Vertical "L" movements
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 1, 2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -1, 2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 1, -2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -1, -2, cap: 1));

        // Horizontal "L" movements
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 2, 1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -2, 1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 2, -1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -2, -1, cap: 1));

        return moves;
    }
}
