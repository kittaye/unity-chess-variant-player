using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceStateSnapshot
{
    public BoardCoord position;
    public bool wasAlive;
    public int moveCount;
    public int captureCount;

    public PieceStateSnapshot(BoardCoord position, bool wasAlive, int moveCount, int captureCount) {
        this.position = position;
        this.wasAlive = wasAlive;
        this.moveCount = moveCount;
        this.captureCount = captureCount;
    }
}
