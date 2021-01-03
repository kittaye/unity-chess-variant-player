using System.Collections.Generic;

public class CoordInfo {
    public string algebraicKey;
    public string file;
    public string rank;
    public object graphicalObject;

    private List<ChessPiece> occupiers;
    private ChessPiece cachedActiveOccupier;


    public CoordInfo(string key) {
        this.algebraicKey = key;
        this.occupiers = new List<ChessPiece>();
        this.graphicalObject = null;
        cachedActiveOccupier = null;

        file = key[0].ToString();
        rank = key.Substring(1);
    }

    public void AddOccupier(ChessPiece piece) {
        occupiers.Add(piece);
    }

    public bool RemoveOccupier(ChessPiece piece) {
        if (cachedActiveOccupier == piece) {
            cachedActiveOccupier = null;
        }
        return occupiers.Remove(piece);
    }

    public ChessPiece GetAliveOccupier() {
        if (cachedActiveOccupier != null && cachedActiveOccupier.IsAlive) {
            return cachedActiveOccupier;
        }

        foreach (ChessPiece piece in occupiers) {
            if (piece.IsAlive) {
                cachedActiveOccupier = piece;
                return piece;
            }
        }
        return null;
    }
}
