using UnityEngine;

public class TileVecConverter
{
    readonly float tileSize;

    public TileVecConverter(float tileSize)
    {
        this.tileSize = tileSize;
    }
    public Vector2 ScreenPositionFromTileVec(TileVec position) => new(position.x * tileSize, -position.y * tileSize);

    public TileVec TileVecFromScreenPosition(Vector2 position)
    {
        Vector2Int vector2Int = Vector2Int.RoundToInt(position / tileSize);
        return new TileVec(vector2Int.x, -vector2Int.y);
    }
}