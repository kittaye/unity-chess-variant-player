using System.Collections.Generic;

public class Pawn : ChessPiece {
    public uint initialMoveLimit;
    public bool canEnPassantCapture;

    public Pawn(Team team, BoardCoord position, Board board) : base(team, position, board) {
        Init();
    }
    public Pawn(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        Init();
    }

    private void Init() {
        m_pieceType = Piece.Pawn;
        this.canEnPassantCapture = true;
        this.initialMoveLimit = 2;
    }

    public override string GetCanonicalName() {
        return "Pawn";
    }

    public override string GetLetterNotation() {
        return string.Empty;
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        uint moveCap = (MoveCount == 0) ? initialMoveLimit : 1;

        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up, moveCap: moveCap, threatAttackLimit: 0));

        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpLeft, moveCap: 1, threatsOnly: true));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpRight, moveCap: 1, threatsOnly: true));

        return moves;
    }
}
