﻿using UnityEngine;
using System.Collections.Generic;

public abstract class ChessPiece {
    private Team m_Team;
    private BoardCoord m_BoardPosition;

    public GameObject gameObject;
    public int MoveCount { get; set; }
    public bool IsAlive { get; set; }

    protected Chess chessGame;

    public ChessPiece(Team team, BoardCoord position) {
        chessGame = GameManager.Instance.chessGame;
        m_Team = team;
        m_BoardPosition = position;
        MoveCount = 0;
        IsAlive = false;
    }

    public abstract List<BoardCoord> CalculateTemplateMoves();

    public BoardCoord GetBoardPosition() {
        return m_BoardPosition;
    }

    public void SetBoardPosition(BoardCoord pos) {
        m_BoardPosition = pos;
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
