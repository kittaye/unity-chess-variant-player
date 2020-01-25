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

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownLeft, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownRight, moveCap: 1));

        // Vertical "L" movements
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 1, 3, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -1, 3, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 1, -3, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -1, -3, moveCap: 1));

        // Horizontal "L" movements
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 3, 1, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -3, 1, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 3, -1, moveCap: 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -3, -1, moveCap: 1));

        return moves;
    }

    public override string GetLetterNotation() {
        return "W";
    }
}
