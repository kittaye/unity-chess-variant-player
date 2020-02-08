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
        int xModifier, yModifier;

        for (int i = 0; i <= 7; i++) {
            chessGame.GetMoveDirectionModifiers(this, (MoveDirection)i, out xModifier, out yModifier);
            BoardCoord coord = GetBoardPosition() + new BoardCoord(xModifier, yModifier);

            while (chessGame.Board.ContainsCoord(coord)) {
                if(chessGame.Board.GetCoordInfo(coord).GetAliveOccupier() != null) {
                    BoardCoord grasshopperMove = chessGame.TryGetSpecificMove(this, coord + new BoardCoord(xModifier, yModifier));
                    if(grasshopperMove != BoardCoord.NULL) {
                        moves.Add(grasshopperMove);
                    }
                    break;
                }
                coord.x += xModifier;
                coord.y += yModifier;
            }
        }
        return moves;
    }

    public override string GetLetterNotation() {
        return "G";
    }
}
