using System.Collections.Generic;

public class Amazon : ChessPiece {
    public Amazon(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Amazon;
    }
    public Amazon(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Amazon;
    }

    public override string ToString() {
        return GetTeam() + "_Amazon";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();
        for (int i = 0; i <= 7; i++) {
            moves.AddRange(chessGame.TryGetDirectionalMoves(this, (MoveDirection)i));
        }

        // Vertical "L" movements
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 1, 2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -1, 2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 1, -2, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -1, -2, cap: 1));

        // Horizontal "L" movements
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 2, 1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -2, 1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, 2, -1, cap: 1));
        moves.AddRange(chessGame.TryGetCustomDirectionalMoves(this, -2, -1, cap: 1));

        return moves;
    }
}
