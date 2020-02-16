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

    public override string GetLetterNotation() {
        return "NR";
    }

    protected override void InitSpecificMoveSet() {
        m_SpecificMoveSet = new BoardCoord[8] {
            // Vertical "L" movements
            new BoardCoord(1, 2),
            new BoardCoord(-1, 2),
            new BoardCoord(1, -2),
            new BoardCoord(-1, -2),

            // Horizontal "L" movements
            new BoardCoord(2, 1),
            new BoardCoord(-2, 1),
            new BoardCoord(2, -1),
            new BoardCoord(-2, -1)
        };
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        // Infinitely extended knight movements
        for (int i = 0; i < m_SpecificMoveSet.Length; i++) {
            moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, m_SpecificMoveSet[i]));
        }

        return moves;
    }
}
