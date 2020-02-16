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

    public override string GetLetterNotation() {
        return "P";
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

        // Bishop movements
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.UpRight));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.UpLeft));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.DownRight));
        moves.AddRange(chessGame.TryGetDirectionalTemplateMoves(this, MoveDirection.DownLeft));

        // Knight movements
        moves.AddRange(TryGetTemplateMovesFromSpecificMoveSet());

        return moves;
    }
}
