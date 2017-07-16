using System.Collections.Generic;

public class King : ChessPiece {
    public King(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.King;
    }

    public override string ToString() {
        return GetTeam() + "_King";
    }
    
    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();
        for (int i = 0; i <= 7; i++) {
            moves.AddRange(chessGame.TryGetDirectionalMoves(this, (MoveDirection)i, 1));
        }
        return moves;
    }
}
