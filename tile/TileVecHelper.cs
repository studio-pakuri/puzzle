using UnityEngine;

public static class TileVecHelper
{
    public static Vector2Int tileVecToVector2Int(TileVec tileVec) => new(tileVec.x, tileVec.y);
    public static TileVec vector2IntToTileVec(Vector2Int vector) => new(vector.x, vector.y);
}