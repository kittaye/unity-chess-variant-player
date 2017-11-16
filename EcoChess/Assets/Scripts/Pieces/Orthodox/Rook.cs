using UnityEngine;
using System.Collections.Generic;

public class Rook : ChessPiece {
    public Rook(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Rook;
    }
    public Rook(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Rook;
    }
    public Rook(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Rook;
    }
    public Rook(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Rook;
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
