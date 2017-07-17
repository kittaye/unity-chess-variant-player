﻿using System.Collections.Generic;

public class Bishop : ChessPiece {
    public Bishop(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Bishop;
    }
    public Bishop(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Bishop;
    }

    public override string ToString() {
        return GetTeam() +"_Bishop";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownLeft));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownRight));

        return moves;
    }
}
