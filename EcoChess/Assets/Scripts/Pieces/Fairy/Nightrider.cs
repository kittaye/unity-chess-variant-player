using UnityEngine;
using System.Collections.Generic;

public class Nightrider : ChessPiece {
    public Nightrider(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Nightrider;
    }
    public Nightrider(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Nightrider;
    }
    public Nightrider(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Nightrider;
    }
    public Nightrider(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Nightrider;
    }

    public override string ToString() {
        return GetTeam() + "_Nightrider";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();
        // Vertical "L" movements
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 1, 2));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -1, 2));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 1, -2));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -1, -2));

        // Horizontal "L" movements
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 2, 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -2, 1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, 2, -1));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, -2, -1));

        return moves;
    }

    public override string GetLetterNotation() {
        return "NR";
    }
}
