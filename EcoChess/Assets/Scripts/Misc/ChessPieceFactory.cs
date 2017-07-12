using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Piece { King, Queen, Knight, Bishop, Rook, Pawn, Empress, Princess, Grasshopper, Nightrider }

public static class ChessPieceFactory {
	public static ChessPiece Create(Piece piece, Team team, BoardCoord position) {
        switch (piece) {
            case Piece.King:
                return new King(team, position);
            case Piece.Queen:
                return new Queen(team, position);
            case Piece.Knight:
                return new Knight(team, position);
            case Piece.Bishop:
                return new Bishop(team, position);
            case Piece.Rook:
                return new Rook(team, position);
            case Piece.Pawn:
                return new Pawn(team, position);

            case Piece.Empress:
                return new Empress(team, position);
            case Piece.Princess:
                return new Princess(team, position);
            case Piece.Grasshopper:
                return new Grasshopper(team, position);
            case Piece.Nightrider:
                return new Nightrider(team, position);
            default:
                Debug.LogError("Piece type: " + piece.ToString() + ", has not been implemented!");
                return null;
        }
    }
}
