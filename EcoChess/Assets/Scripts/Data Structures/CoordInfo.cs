using UnityEngine;
using System.Collections;

public class CoordInfo {
    public readonly string algebraicKey;
    public GameObject boardChunk;
    private ChessPiece m_occupier;
    public ChessPiece occupier {
        get {
            return m_occupier;
        }
        set {
            m_occupier = value;
        }
    }

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
