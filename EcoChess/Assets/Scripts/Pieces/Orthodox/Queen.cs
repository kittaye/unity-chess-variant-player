
public class Queen : ChessPiece {
    public Queen(Team team, BoardCoord position) : base(team, position) {

    }

    public override string ToString() {
        return GetTeam() + "_Queen";
    }

    public override void CalculateTemplateMoves() {
        for (int i = 0; i <= 7; i++) {
            AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, (MoveDirection)i));
        }
    }
}
