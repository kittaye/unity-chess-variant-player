﻿using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece {
    public readonly uint initialMoveLimit;
    public readonly bool canEnPassantCapture;
    public bool validEnPassant;

    public Pawn(Team team, BoardCoord position, bool canEnPassantCapture = true, uint initialMoveLimit = 2) : base(team, position) {
        m_pieceType = Piece.Pawn;
        this.validEnPassant = false;
        this.canEnPassantCapture = canEnPassantCapture;
        this.initialMoveLimit = initialMoveLimit;
    }
    public Pawn(Team team, string algebraicKeyPosition, bool canEnPassantCapture = true, uint initialMoveLimit = 2) : base(team, algebraicKeyPosition) {
        m_pieceType = Piece.Pawn;
        this.validEnPassant = false;
        this.canEnPassantCapture = canEnPassantCapture;
        this.initialMoveLimit = initialMoveLimit;
    }
    public Pawn(Team team, BoardCoord position, bool allowXWrapping, bool allowYWrapping, bool canEnPassantCapture = true, uint initialMoveLimit = 2) 
        : base(team, position, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Pawn;
        this.validEnPassant = false;
        this.canEnPassantCapture = canEnPassantCapture;
        this.initialMoveLimit = initialMoveLimit;
    }
    public Pawn(Team team, string algebraicKeyPosition, bool allowXWrapping, bool allowYWrapping, bool canEnPassantCapture = true, uint initialMoveLimit = 2)
    : base(team, algebraicKeyPosition, allowXWrapping, allowYWrapping) {
        m_pieceType = Piece.Pawn;
        this.validEnPassant = false;
        this.canEnPassantCapture = canEnPassantCapture;
        this.initialMoveLimit = initialMoveLimit;
    }

    public override string ToString() {
        return GetTeam() + "_Pawn";
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        uint moveCap = (MoveCount == 0) ? initialMoveLimit : 1;

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.Up, cap: moveCap, threatAttackLimit: 0));

        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpLeft, cap: 1, threatsOnly: true));
        moves.AddRange(chessGame.TryGetDirectionalMoves(this, MoveDirection.UpRight, cap: 1, threatsOnly: true));

        return moves;
    }
}
