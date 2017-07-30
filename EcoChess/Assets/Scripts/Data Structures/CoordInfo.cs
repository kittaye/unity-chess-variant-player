using UnityEngine;
using System.Collections;

public class CoordInfo {
    public string algebraicKey;
    public readonly GameObject boardChunk;
    public ChessPiece occupier;

    public CoordInfo(string key, GameObject boardChunk, ChessPiece pieceOccupee = null) {
        this.algebraicKey = key;
        this.occupier = pieceOccupee;
        this.boardChunk = boardChunk;
    }

    public CoordInfo(string key, ChessPiece pieceOccupee = null) {
        this.algebraicKey = key;
        this.occupier = pieceOccupee;
        this.boardChunk = null;
    }
}
