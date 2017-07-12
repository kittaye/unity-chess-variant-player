using UnityEngine;
using System.Collections.Generic;

public class Grasshopper : ChessPiece {
    public Grasshopper(Team team, BoardCoord position) : base(team, position) {
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

            while (chessGame.board.ContainsCoord(coord)) {
                if(chessGame.board.GetCoordInfo(coord).occupier != null) {
                    moves.Add(chessGame.TryGetSpecificMove(this, coord + new BoardCoord(xModifier, yModifier)));
                    break;
                }
                coord.x += xModifier;
                coord.y += yModifier;
            }
        }
        return moves;
    }
}
