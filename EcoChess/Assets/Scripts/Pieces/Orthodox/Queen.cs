﻿using System.Collections.Generic;

public class Queen : ChessPiece {
    public Queen(Team team, BoardCoord position) : base(team, position) {

    }

    public override string ToString() {
        return GetTeam() + "_Queen";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();
        for (int i = 0; i <= 7; i++) {
            moves.AddRange(chessGame.TryGetDirectionalMoves(this, (MoveDirection)i));
        }
        return moves;
    }
}
