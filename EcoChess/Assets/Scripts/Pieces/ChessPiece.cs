using System.Collections.Generic;

public abstract class ChessPiece {
    public object graphicalObject;
    public int MoveCount { get; set; }
    public int CaptureCount { get; set; }
    public bool IsAlive { get; set; }
    public bool HasXWrapping { get; set; }
    public bool HasYWrapping { get; set; }
    public List<PieceStateSnapshot> StateHistory { get; set; }

    protected Piece m_pieceType;
    protected BoardCoord[] m_SpecificMoveSet;
    protected Board m_Board;

    private Team m_Team;
    private BoardCoord m_BoardPosition;

    public ChessPiece(Team team, BoardCoord position, Board board) {
        Init(team, position, board);
    }

    public ChessPiece(Team team, string algebraicKeyPosition, Board board) {
        if (m_Board.TryGetCoordWithKey(algebraicKeyPosition, out BoardCoord position)) {
            Init(team, position, board);
        }
    }

    private void Init(Team team, BoardCoord position, Board board) {
        m_Board = board;

        MoveCount = 0;
        IsAlive = false;
        StateHistory = new List<PieceStateSnapshot>();
        InitSpecificMoveSet();

        HasXWrapping = false;
        HasYWrapping = false;

        m_Team = team;
        m_BoardPosition = position;
    }

    public void UpdateStateHistory() {
        StateHistory.Add(new PieceStateSnapshot(m_BoardPosition, IsAlive, MoveCount, CaptureCount));
    }

    protected virtual void InitSpecificMoveSet() {
        m_SpecificMoveSet = new BoardCoord[0];
    }

    public bool IsThreatTowards(ChessPiece target) {
        return target.GetTeam() != this.GetTeam() && target.GetTeam() != Team.NONE;
    }

    public bool IsAllyTowards(ChessPiece target) {
        return !IsThreatTowards(target) && target.GetTeam() != Team.NONE;
    }

    public bool IsThreatTowards(BoardCoord coord) {
        ChessPiece occupier = m_Board.GetCoordInfo(coord).GetAliveOccupier();
        if (occupier != null) {
            return IsThreatTowards(occupier);
        }
        return false;
    }

    public bool IsAllyTowards(BoardCoord coord) {
        ChessPiece occupier = m_Board.GetCoordInfo(coord).GetAliveOccupier();
        if (occupier != null) {
            return IsAllyTowards(occupier);
        }
        return false;
    }

    public abstract List<BoardCoord> CalculateTemplateMoves();

    public abstract string GetLetterNotation();

    public abstract string GetCanonicalName();

    public BoardCoord GetBoardPosition() {
        return m_BoardPosition;
    }

    public void SetBoardPosition(BoardCoord pos) {
        m_BoardPosition = pos;
    }

    public override string ToString() {
        return GetTeam() + "_" + GetCanonicalName();
    }

    public Piece GetPieceType() {
        return m_pieceType;
    }

    public Team GetTeam() {
        return m_Team;
    }

    /// <summary>
    /// Sets the piece's team. This is only needed for sovereign chess where neutral pieces exist.
    /// </summary>
    public void SetTeam(Team team) {
        m_Team = team;
    }

    public Team GetOpposingTeam() {
        return (GetTeam() == Team.WHITE) ? Team.BLACK : Team.WHITE;
    }

    public BoardCoord GetRelativeBoardCoord(int x, int y) {
        return new BoardCoord(GetBoardPosition().x + TeamSensitiveMove(x), GetBoardPosition().y + TeamSensitiveMove(y));
    }

    public BoardCoord GetRelativeBoardCoord(BoardCoord coord) {
        return new BoardCoord(GetBoardPosition().x + TeamSensitiveMove(coord.x), GetBoardPosition().y + TeamSensitiveMove(coord.y));
    }

    public int TeamSensitiveMove(int x) {
        return (GetTeam() == Team.WHITE) ? x : -x;
    }

    protected List<BoardCoord> TryGetTemplateMovesFromSpecificMoveSet(bool threatsOnly = false) {
        List<BoardCoord> moves = new List<BoardCoord>();

        for (int i = 0; i < m_SpecificMoveSet.Length; i++) {
            BoardCoord relativeSpecificCoord = GetRelativeBoardCoord(m_SpecificMoveSet[i]);

            if (m_Board.ContainsCoord(relativeSpecificCoord)) {
                if ((threatsOnly && this.IsThreatTowards(relativeSpecificCoord)) || !this.IsAllyTowards(relativeSpecificCoord)) {
                    moves.Add(relativeSpecificCoord);
                }
            }
        }

        return moves;
    }

    /// <summary>
    /// Gets a list of moves that a chess piece can move to. This method is used to build up a chess piece's list of template moves.
    /// </summary>
    /// <param name="moveCap">Number of squares to test before stopping (0 = unbounded).</param>
    /// <returns></returns>
    public BoardCoord[] TryGetDirectionalTemplateMoves(MoveDirection direction, uint moveCap = 0, uint threatAttackLimit = 1, bool threatsOnly = false, bool teamSensitive = true) {
        BoardCoord coordStep = GetCoordStepInDirection(direction, teamSensitive);
        return GetMovesInStepPattern(coordStep, moveCap, threatAttackLimit, threatsOnly);
    }

    /// <summary>
    /// Gets a list of moves that a chess piece can move to. This method is used to build up a chess piece's list of template moves.
    /// </summary>
    /// <param name="moveCap">Number of squares to test before stopping (0 = unbounded).</param>
    /// <returns></returns>
    public BoardCoord[] TryGetDirectionalTemplateMoves(BoardCoord coordStep, uint moveCap = 0, uint threatAttackLimit = 1, bool threatsOnly = false, bool teamSensitive = true) {
        if (teamSensitive) {
            coordStep = new BoardCoord(this.TeamSensitiveMove(coordStep.x), this.TeamSensitiveMove(coordStep.y));
        }
        return GetMovesInStepPattern(coordStep, moveCap, threatAttackLimit, threatsOnly);
    }


    /// <summary>
    /// Loops indefinitely in a step pattern to get moves for a piece.
    /// </summary>
    public BoardCoord[] GetMovesInStepPattern(BoardCoord coordStep, uint cap = 0, uint threatAttackLimit = 1, bool threatsOnly = false) {
        uint iter = 0;
        uint threats = 0;
        List<BoardCoord> moves = new List<BoardCoord>();
        BoardCoord coord = this.GetBoardPosition();

        while (true) {
            coord += coordStep;
            if (this.HasXWrapping) coord.x = MathExtensions.mod(coord.x, m_Board.GetWidth());
            if (this.HasYWrapping) coord.y = MathExtensions.mod(coord.y, m_Board.GetHeight());

            if (m_Board.ContainsCoord(coord) == false) break;

            ChessPiece occupier = m_Board.GetCoordInfo(coord).GetAliveOccupier();
            if (occupier != null) {
                if (this.IsAllyTowards(occupier)) {
                    break;
                } else if (this.IsThreatTowards(occupier)) {
                    if (threatAttackLimit == 0) break;
                    moves.Add(coord);
                    threats++;
                    if (threats == threatAttackLimit) break;
                }
            } else {
                if (threatsOnly) break;
                moves.Add(coord);
            }

            iter++;
            if (iter == cap) break;
        }

        return moves.ToArray();
    }

    /// <summary>
    /// Returns the x and y step values for a specific move direction.
    /// </summary>
    public BoardCoord GetCoordStepInDirection(MoveDirection direction, bool teamSensitive) {
        int xStep;
        int yStep;

        switch (direction) {
            case MoveDirection.Up:
                xStep = 0;
                yStep = (teamSensitive) ? TeamSensitiveMove(1) : 1;
                break;                    
            case MoveDirection.Down:      
                xStep = 0;                
                yStep = (teamSensitive) ? TeamSensitiveMove(-1) : -1;
                break;                    
            case MoveDirection.Left:      
                xStep = (teamSensitive) ? TeamSensitiveMove(-1) : -1;
                yStep = 0;                
                break;                    
            case MoveDirection.Right:     
                xStep = (teamSensitive) ? TeamSensitiveMove(1) : 1;
                yStep = 0;                
                break;                    
            case MoveDirection.UpLeft:    
                xStep = (teamSensitive) ? TeamSensitiveMove(-1) : -1;
                yStep = (teamSensitive) ? TeamSensitiveMove(1) : 1;
                break;                    
            case MoveDirection.UpRight:   
                xStep = (teamSensitive) ? TeamSensitiveMove(1) : 1;
                yStep = (teamSensitive) ? TeamSensitiveMove(1) : 1;
                break;                    
            case MoveDirection.DownLeft:  
                xStep = (teamSensitive) ? TeamSensitiveMove(-1) : -1;
                yStep = (teamSensitive) ? TeamSensitiveMove(-1) : -1;
                break;                    
            case MoveDirection.DownRight: 
                xStep = (teamSensitive) ? TeamSensitiveMove(1) : 1;
                yStep = (teamSensitive) ? TeamSensitiveMove(-1) : -1;
                break;
            default:
                xStep = 0;
                yStep = 0;
                break;
        }

        return new BoardCoord(xStep, yStep);
    }
}
