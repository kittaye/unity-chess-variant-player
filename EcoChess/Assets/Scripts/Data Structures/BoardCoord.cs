using UnityEngine;
using System.Collections;
using System;

public struct BoardCoord {
    public int x;
    public int y;
    public static BoardCoord NULL = new BoardCoord(-1, -1);

    public BoardCoord(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public override string ToString() {
        return string.Format("({0}, {1})", x, y);
    }

    public override bool Equals(object obj) {
        if (!(obj is BoardCoord)) return false;
        if (this.x == ((BoardCoord)obj).x && this.y == ((BoardCoord)obj).y) return true;
        return false;
    }

    public static bool operator ==(BoardCoord a, BoardCoord b) {
        return a.Equals(b);
    }

    public static bool operator !=(BoardCoord a, BoardCoord b) {
        return !a.Equals(b);
    }

    public static BoardCoord operator +(BoardCoord a, BoardCoord b) {
        return new BoardCoord(a.x + b.x, a.y + b.y);
    }

    public static BoardCoord operator -(BoardCoord a, BoardCoord b) {
        return new BoardCoord(a.x - b.x, a.y - b.y);
    }

    public static explicit operator BoardCoord(Vector3 v) {
        return new BoardCoord((int)v.x, (int)v.y);
    }

    public static implicit operator Vector3(BoardCoord v) {
        return new Vector3(v.x, v.y, -1);
    }

    public override int GetHashCode() {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            hash *= 486187739 + x.GetHashCode();
            hash *= 486187739 + y.GetHashCode();
            return hash;
        }
    }
}
