using System.Collections.Generic;

public class Grasshopper : ChessPiece {
    public Grasshopper(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.Grasshopper;
    }
    public Grasshopper(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.Grasshopper;
    }

    public override string GetCanonicalName() {
        return "Grasshopper";
    }

    public override string GetLetterNotation() {
        return "G";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        for (int i = 0; i <= 7; i++) {
            BoardCoord coordStep = this.GetCoordStepInDirection((MoveDirection)i, true);
            BoardCoord coord = this.GetBoardPosition() + coordStep;

            while (m_Board.ContainsCoord(coord)) {
                if (m_Board.GetCoordInfo(coord).GetAliveOccupier() != null) {
                    BoardCoord hopMove = coord + coordStep;

                    if (m_Board.ContainsCoord(hopMove)) {
                        moves.Add(hopMove);
                        break;
                    }
                }

                coord += coordStep;
            }
        }

        return moves;
    }
}
