using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamTurnState
{
    public Team currentTeam;
    public Team opposingTeam;
    public ChessPiece currentRoyalPiece;
    public ChessPiece opposingRoyalPiece;
    // TODO: change name to gameturnstate and add numconseqcaptureless field

    public TeamTurnState(Team currentTeam, Team opposingTeam, ChessPiece currentRoyalPiece, ChessPiece opposingRoyalPiece) {
        this.currentTeam = currentTeam;
        this.opposingTeam = opposingTeam;
        this.currentRoyalPiece = currentRoyalPiece;
        this.opposingRoyalPiece = opposingRoyalPiece;
    }
}
