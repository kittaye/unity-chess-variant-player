using UnityEngine;
using System.Collections.Generic;

public class Nightrider : ChessPiece {
    public Nightrider(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Nightrider;
    }

    public override string ToString() {
        return GetTeam() + "_Nightrider";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();
        // Vertical "L" movements
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 1, 2));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -1, 2));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 1, -2));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -1, -2));

        // Horizontal "L" movements
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 2, 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -2, 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 2, -1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -2, -1));

        return moves;
    }
}
