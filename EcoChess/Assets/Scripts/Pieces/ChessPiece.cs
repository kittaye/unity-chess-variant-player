using UnityEngine;
using System.Collections.Generic;

public abstract class ChessPiece {
    private Team m_Team;
    private BoardCoord m_BoardPosition;

    protected Piece m_pieceType;
    public GameObject gameObject;
    public int MoveCount { get; private set; }
    public int CaptureCount { get; set; }
    public bool IsAlive { get; set; }
    public bool hasXWrapping { get; set; }
    public bool hasYWrapping { get; set; }

    protected ChessGameModes.Chess chessGame;

    public ChessPiece(Team team, BoardCoord position) {
        chessGame = GameManager.Instance.ChessGame;
        m_Team = team;
        m_BoardPosition = position;
        MoveCount = 0;
        IsAlive = false;
        hasXWrapping = false;
        hasYWrapping = false;
    }

    public ChessPiece(Team team, string algebraicKeyPosition) {
        chessGame = GameManager.Instance.ChessGame;
        m_Team = team;
        BoardCoord position = BoardCoord.NULL;
        if(chessGame.board.TryGetCoordWithKey(algebraicKeyPosition, out position)) {
            m_BoardPosition = position;
        }
        MoveCount = 0;
        IsAlive = false;
        hasXWrapping = false;
        hasYWrapping = false;
    }

    public ChessPiece(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping) {
        chessGame = GameManager.Instance.ChessGame;
        m_Team = team;
        m_BoardPosition = position;
        MoveCount = 0;
        IsAlive = false;
        this.hasXWrapping = allowXWrapping;
        this.hasYWrapping = allowYWrapping;
    }

    public ChessPiece(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping) {
        chessGame = GameManager.Instance.ChessGame;
        m_Team = team;
        BoardCoord position = BoardCoord.NULL;
        if (chessGame.board.TryGetCoordWithKey(algebraicKeyPosition, out position)) {
            m_BoardPosition = position;
        }
        MoveCount = 0;
        IsAlive = false;
        this.hasXWrapping = allowXWrapping;
        this.hasYWrapping = allowYWrapping;
    }

    public abstract List<BoardCoord> CalculateTemplateMoves();

    public BoardCoord GetBoardPosition() {
        return m_BoardPosition;
    }

    public void SetBoardPosition(BoardCoord pos) {
        m_BoardPosition = pos;
    }

    public Piece GetPieceType() {
        return m_pieceType;
    }

    public bool MakeMoveTo(BoardCoord destination) {
        if (chessGame.AssertContainsCoord(destination)) {
            SetBoardPosition(destination);
            gameObject.transform.position = destination;
            MoveCount++;
            return true;
        }
        return false;
    }

    public Team GetTeam() {
        return m_Team;
    }

    public Team GetOpposingTeam() {
        return (GetTeam() == Team.WHITE) ? Team.BLACK : Team.WHITE;
    }

    public BoardCoord GetRelativeBoardCoord(int x, int y) {
        return new BoardCoord(GetBoardPosition().x + TeamSensitiveMove(x), GetBoardPosition().y + TeamSensitiveMove(y));
    }

    public int TeamSensitiveMove(int x) {
        return (GetTeam() == Team.WHITE) ? x : -x;
    }
}
