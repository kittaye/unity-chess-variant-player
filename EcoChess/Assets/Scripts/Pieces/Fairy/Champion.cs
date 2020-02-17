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

    public override string GetCanonicalName() {
        return "Champion";
    }

    public override string GetLetterNotation() {
        return "C";
    }

    protected override void InitSpecificMoveSet() {
        m_SpecificMoveSet = new BoardCoord[12] {
            new BoardCoord(0, 1),
            new BoardCoord(0, -1),
            new BoardCoord(1, 0),
            new BoardCoord(-1, 0),

            new BoardCoord(0, 2),
            new BoardCoord(0, -2),
            new BoardCoord(2, 0),
            new BoardCoord(-2, 0),

            new BoardCoord(2, 2),
            new BoardCoord(-2, 2),
            new BoardCoord(2, -2),
            new BoardCoord(-2, -2)
        };
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>(12);

        moves.AddRange(TryGetTemplateMovesFromSpecificMoveSet());

        return moves;
    }
}
