using UnityEngine;
using System.Collections.Generic;

public class Grasshopper : ChessPiece {
    public Grasshopper(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Grasshopper;
    }
    public Grasshopper(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Grasshopper;
    }
    public Grasshopper(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Grasshopper;
    }
    public Grasshopper(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Grasshopper;
    }

    public override string ToString() {
        return GetTeam() + "_Grasshopper";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        for (int i = 0; i <= 7; i++) {
            BoardCoord coordStep = chessGame.GetCoordStepInDirection(this, (MoveDirection)i, true);
            BoardCoord coord = GetBoardPosition() + coordStep;

            while (chessGame.Board.ContainsCoord(coord)) {
                if (chessGame.Board.GetCoordInfo(coord).GetAliveOccupier() != null) {
                    BoardCoord hopMove = coord + coordStep;

                    if (chessGame.Board.ContainsCoord(hopMove)) {
                        moves.Add(hopMove);
                        break;
                    }
                }

                coord += coordStep;
            }
        }

        return moves;
    }

    public override string GetLetterNotation() {
        return "G";
    }
}
