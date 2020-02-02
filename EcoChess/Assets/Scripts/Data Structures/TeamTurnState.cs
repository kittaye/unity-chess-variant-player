using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamTurnState
{
    public Team currentTeam;
    public Team opposingTeam;
    public ChessPiece currentRoyalPiece;
    public ChessPiece opposingRoyalPiece;
    public ChessPiece lastMovedWhitePiece;
    public ChessPiece lastMovedBlackPiece;
    public uint numConsequtiveCapturelessMoves;

    public TeamTurnState(Team currentTeam, Team opposingTeam, ChessPiece currentRoyalPiece, ChessPiece opposingRoyalPiece, 
        ChessPiece lastMovedWhitePiece, ChessPiece lastMovedBlackPiece, uint conseqCaptureless) {
        this.currentTeam = currentTeam;
        this.opposingTeam = opposingTeam;
        this.currentRoyalPiece = currentRoyalPiece;
        this.opposingRoyalPiece = opposingRoyalPiece;
        this.lastMovedWhitePiece = lastMovedWhitePiece;
        this.lastMovedBlackPiece = lastMovedBlackPiece;
        this.numConsequtiveCapturelessMoves = conseqCaptureless;
    }
}
