
public class Bishop : ChessPiece {
    public Bishop(Team team, BoardCoord position) : base(team, position) {

    }

    public override string ToString() {
        return GetTeam() +"_Bishop";
    }

    public override void CalculateTemplateMoves() {
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight));
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft));
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownLeft));
        AddTemplateMoves(chessGame.TryGetDirectionalMoves(this, MoveDirection.DownRight));
    }
}
