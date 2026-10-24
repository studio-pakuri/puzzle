using System;
using UnityEditor.Experimental.GraphView;

public readonly struct TileVec : IEquatable<TileVec>
{
    // UnityEngine.Vector2Intが可変なのが気に入らないから
    public TileVec(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public readonly int x;
    public readonly int y;
    public static bool operator ==(TileVec a, TileVec b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(TileVec a, TileVec b) => a.x != b.x || a.y != b.y;
    public static TileVec operator +(TileVec a, TileVec b) => new(a.x + b.x, a.y + b.y);
    public static TileVec operator -(TileVec a, TileVec b) => new(a.x - b.x, a.y - b.y);
    public static TileVec operator -(TileVec tileVec) => new(-tileVec.x, -tileVec.y);
    public static TileVec operator *(TileVec tileVec, int value) => new(tileVec.x * value, tileVec.y * value);
    public static implicit operator TileVec(TileDirection d) => d switch
    {
        TileDirection.UP => new TileVec(0, 1),
        TileDirection.DOWN => new TileVec(0, -1),
        TileDirection.RIGHT => new TileVec(1, 0),
        TileDirection.LEFT => new TileVec(-1, 0)
    };
    public override bool Equals(object obj)
    {
        if (obj is not TileVec tileVec) return false;
        return x == tileVec.x && y == tileVec.y;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public bool Equals(TileVec other) => x == other.x && y == other.y;
}