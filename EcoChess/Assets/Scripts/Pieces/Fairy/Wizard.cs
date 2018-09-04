using UnityEngine;
using System.Collections.Generic;

public class Wizard : ChessPiece {
    public Wizard(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Wizard;
    }
    public Wizard(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Wizard;
    }
    public Wizard(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Wizard;
    }
    public Wizard(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Wizard;
    }

    public override string ToString() {
        return GetTeam() + "_Wizard";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft, cap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight, cap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownLeft, cap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownRight, cap: 1));

        // Vertical "L" movements
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 1, 3, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -1, 3, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 1, -3, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -1, -3, cap: 1));

        // Horizontal "L" movements
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 3, 1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -3, 1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 3, -1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -3, -1, cap: 1));

        return moves;
    }

    public override string GetLetterNotation() {
        return "W";
    }
}
