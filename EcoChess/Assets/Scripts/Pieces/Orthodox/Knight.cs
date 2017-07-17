using System.Collections.Generic;

public class Knight : ChessPiece {
    public Knight(Team team, BoardCoord position) : base(team, position) {
        m_pieceType = Piece.Knight;
    }
    public Knight(Team team, string algebraicKeyPosition) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Knight;
    }

    public override string ToString() {
        return GetTeam() + "_Knight";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();
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
