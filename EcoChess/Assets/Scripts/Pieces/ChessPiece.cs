using UnityEngine;
using System.Collections.Generic;

public abstract class ChessPiece {
    private Team m_Team;
    private HashSet<BoardCoord> m_TemplateMoves;
    private HashSet<BoardCoord> m_AvailableMoves;
    private BoardCoord m_BoardPosition;

    public GameObject gameObject;
    public int MoveCount { get; set; }
    public bool IsAlive { get; set; }

    protected Chess chessGame;

    public ChessPiece(Team team, BoardCoord position) {
        chessGame = GameManager.Instance.chessGame;
        m_Team = team;
        m_BoardPosition = position;
        m_TemplateMoves = new HashSet<BoardCoord>();
        m_AvailableMoves = new HashSet<BoardCoord>();
        MoveCount = 0;
        IsAlive = false;
    }

    public abstract void CalculateTemplateMoves();

    public BoardCoord GetBoardPosition() {
        return m_BoardPosition;
    }

    public void SetBoardPosition(BoardCoord pos) {
        m_BoardPosition = pos;
    }

    public void ClearAvailableMoves() {
        m_AvailableMoves.Clear();
    }

    public virtual void ClearTemplateMoves() {
        m_TemplateMoves.Clear();
    }

    public bool MakeMoveTo(BoardCoord destination) {
        if (chessGame.AssertContainsCoord(destination) && CanMoveTo(destination)) {
            SetBoardPosition(destination);
            gameObject.transform.position = destination;
            MoveCount++;
            return true;
        }
        return false;
    }

    public bool CanMoveTo(BoardCoord destination) {
        return m_AvailableMoves.Contains(destination);
    }

    public bool HasTemplateMoveTo(BoardCoord destination) {
        return m_TemplateMoves.Contains(destination);
    }

    public void AddTemplateMoves(BoardCoord[] coords) {
        foreach (BoardCoord coord in coords) {
            if (chessGame.board.ContainsCoord(coord)) {
                m_TemplateMoves.Add(coord);
            }
        }
    }

    public void AddTemplateMoves(BoardCoord coord) {
        if (chessGame.board.ContainsCoord(coord)) {
            m_TemplateMoves.Add(coord);
        }
    }

    public void AddToAvailableMoves(BoardCoord coord) {
        if (chessGame.board.ContainsCoord(coord)) {
            m_AvailableMoves.Add(coord);
        }
    }

    public void AddToAvailableMoves(BoardCoord[] coords) {
        foreach (BoardCoord coord in coords) {
            if (chessGame.board.ContainsCoord(coord)) {
                m_AvailableMoves.Add(coord);
            }
        }
    }

    public void RemoveAvailableMoves(BoardCoord coord) {
        if (chessGame.board.ContainsCoord(coord)) {
            m_AvailableMoves.Remove(coord);
        }
    }

    public BoardCoord[] GetTemplateMoves() {
        return new List<BoardCoord>(m_TemplateMoves).ToArray();
    }

    public BoardCoord[] GetAvailableMoves() {
        return new List<BoardCoord>(m_AvailableMoves).ToArray();
    }

    public Team GetTeam() {
        return m_Team;
    }

    public BoardCoord GetRelativeBoardCoord(int x, int y) {
        return new BoardCoord(GetBoardPosition().x + TeamSensitiveMove(x), GetBoardPosition().y + TeamSensitiveMove(y));
    }

    public int TeamSensitiveMove(int x) {
        return (GetTeam() == Team.WHITE) ? x : -x;
    }
}
