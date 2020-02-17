using UnityEngine;
using System.Collections.Generic;

public abstract class ChessPiece {
    private Team m_Team;
    private BoardCoord m_BoardPosition;

    protected Piece m_pieceType;
    protected BoardCoord[] m_SpecificMoveSet;

    public GameObject gameObject;
    public int MoveCount { get; set; }
    public int CaptureCount { get; set; }
    public bool IsAlive { get; set; }
    public bool HasXWrapping { get; private set; }
    public bool HasYWrapping { get; private set; }
    public List<PieceStateSnapshot> StateHistory { get; set; }

    protected ChessGameModes.Chess chessGame;

    public ChessPiece(Team team, BoardCoord position) {
        Init();

        m_Team = team;
        m_BoardPosition = position;
        HasXWrapping = false;
        HasYWrapping = false;
    }

    public ChessPiece(Team team, string algebraicKeyPosition) {
        Init();

        m_Team = team;
        BoardCoord position = BoardCoord.NULL;
        if(chessGame.Board.TryGetCoordWithKey(algebraicKeyPosition, out position)) {
            m_BoardPosition = position;
        }
        HasXWrapping = false;
        HasYWrapping = false;
    }

    public ChessPiece(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) {
        Init();

        m_Team = team;
        m_BoardPosition = position;
        this.HasXWrapping = allowXWrapping;
        this.HasYWrapping = allowYWrapping;
    }

    public ChessPiece(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping) {
        Init();

        m_Team = team;
        BoardCoord position = BoardCoord.NULL;
        if (chessGame.Board.TryGetCoordWithKey(algebraicKeyPosition, out position)) {
            m_BoardPosition = position;
        }
        this.HasXWrapping = allowXWrapping;
        this.HasYWrapping = allowYWrapping;
    }

    private void Init() {
        chessGame = GameManager.Instance.ChessGame;
        MoveCount = 0;
        IsAlive = false;
        StateHistory = new List<PieceStateSnapshot>();
        InitSpecificMoveSet();
    }

    public void UpdateStateHistory() {
        StateHistory.Add(new PieceStateSnapshot(m_BoardPosition, IsAlive, MoveCount, CaptureCount));
    }

    protected virtual void InitSpecificMoveSet() {
        m_SpecificMoveSet = new BoardCoord[0];
    }

    protected List<BoardCoord> TryGetTemplateMovesFromSpecificMoveSet(bool threatsOnly = false) {
        List<BoardCoord> moves = new List<BoardCoord>();

        for (int i = 0; i < m_SpecificMoveSet.Length; i++) {
            BoardCoord relativeSpecificMove = GetRelativeBoardCoord(m_SpecificMoveSet[i]);

            if (chessGame.Board.ContainsCoord(relativeSpecificMove)) {
                if ((threatsOnly && chessGame.IsThreat(this, relativeSpecificMove)) || chessGame.IsAlly(this, relativeSpecificMove) == false) {
                    moves.Add(relativeSpecificMove);
                }
            }
        }

        return moves;
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
}
