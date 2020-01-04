using UnityEngine;
using System.Collections;

public class CoordInfo {
    public string algebraicKey;
    public string file;
    public string rank;
    public readonly GameObject boardChunk;
    public ChessPiece occupier;

    public CoordInfo(string key, GameObject boardChunk, ChessPiece pieceOccupee = null) {
        this.algebraicKey = key;
        this.occupier = pieceOccupee;
        this.boardChunk = boardChunk;

        file = key[0].ToString();
        rank = key.Substring(1);
    }

    public CoordInfo(string key, ChessPiece pieceOccupee = null) {
        this.algebraicKey = key;
        this.occupier = pieceOccupee;
        this.boardChunk = null;

        file = key[0].ToString();
        rank = key.Substring(1);
    }
}
