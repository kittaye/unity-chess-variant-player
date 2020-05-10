using System.Collections.Generic;

/// <summary>
/// Legan Pawns are specific to the Legan Chess variant, but could be used elsewhere.
/// </summary>
public class LeganPawn : Pawn {

    public LeganPawn(Team team, BoardCoord position, Board board) : base(team, position, board) {
        Init();
    }
    public LeganPawn(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        Init();
    }

    private void Init() {
        m_pieceType = Piece.LeganPawn;
        canEnPassantCapture = false;
        initialMoveLimit = 1;
    }

    public override string GetCanonicalName() {
        return "Pawn";
    }

    public override string GetLetterNotation() {
        return "LP";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpLeft, moveCap: 1, threatAttackLimit: 0));

        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up, moveCap: 1, threatsOnly: true));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Left, moveCap: 1, threatsOnly: true));

        return moves;
    }
}
