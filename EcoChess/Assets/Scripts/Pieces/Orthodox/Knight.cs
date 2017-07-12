
public class Knight : ChessPiece {
    public Knight(Team team, BoardCoord position) : base(team, position) {

    }

    public override string ToString() {
        return GetTeam() + "_Knight";
    }

    public override void CalculateTemplateMoves() {
        // Vertical "L" movements
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, 1, 2, cap: 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, -1, 2, cap: 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, 1, -2, cap: 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, -1, -2, cap: 1));

        // Horizontal "L" movements
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, 2, 1, cap: 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, -2, 1, cap: 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, 2, -1, cap: 1));
        AddTemplateMoves(chessGame.TryGetCustomDirectionalMoves(this, -2, -1, cap: 1));
    }
}
