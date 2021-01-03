using System.Collections.Generic;

public class BerolinaPawn : Pawn {
    public BerolinaPawn(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.BerolinaPawn;
    }
    public BerolinaPawn(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.BerolinaPawn;
    }

    public override string GetCanonicalName() {
        return "BerolinaPawn";
    }

    public override string GetLetterNotation() {
        return "BP";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        uint moveCap = (MoveCount == 0) ? initialMoveLimit : 1;

        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpRight, moveCap: moveCap, threatAttackLimit: 0));
        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpLeft, moveCap: moveCap, threatAttackLimit: 0));

        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up, moveCap: 1, threatsOnly: true));

        return moves;
    }
}
