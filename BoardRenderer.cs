using System.Collections.Generic;
using UnityEngine;

public class BoardRenderer: MonoBehaviour
{
    public int rows;
    public int columns;
    float tileSize;
    Vector2 centerPosition;
    List<GameObject> blocks = new();
    GameObject player;
    [SerializeField] GameObject[] prefabs;
    public Vector2 GetScreenPositionFromTileTable(TileVec position)
    {
        return new Vector2(position.x * tileSize - centerPosition.x, -position.y * tileSize + centerPosition.y);
    }

    public void CreateStage(FloorLayer floorLayer, IBoardView boardView)
    {
        tileSize = prefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
        centerPosition.x = rows / 2 * tileSize;
        centerPosition.y = columns / 2 * tileSize;
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                TileVec tileVec = new(y, x);
                Vector2 position = GetScreenPositionFromTileTable(tileVec);
                FloorLayer.FloorLayerTile? tile = floorLayer.GetTile(tileVec);
                if (tile is not null){
                    GameObject groundObj = Instantiate(prefabs[tile switch
                    {
                        FloorLayer.FloorLayerTile.WALL => (int)StageManager.StageType.WALL,
                        FloorLayer.FloorLayerTile.FLOOR => (int)StageManager.StageType.GROUND,
                        FloorLayer.FloorLayerTile.POINT => (int)StageManager.StageType.BLOCK_POINT
                    }]);
                    groundObj.transform.position = position;
                }
                if (boardView.HasObject(tileVec))
                {
                    BoardObjectType type = boardView.GetObject(tileVec).type;
                    if (type == BoardObjectType.BLOCK)
                    {
                        GameObject block = Instantiate(prefabs[3]);
                        blocks.Add(block);
                    }
                }
            }
        }
        player = Instantiate(prefabs[4]);
        UpdateStage(boardView);
    }

    public void UpdateStage(IBoardView boardView)
    {
        int i = 0;
        foreach (var (tileVec, boardObject) in boardView)
        {
            switch (boardObject.type)
            {
                case BoardObjectType.PLAYER:
                    player.transform.position = GetScreenPositionFromTileTable(tileVec);
                    break;
                case BoardObjectType.BLOCK:
                    blocks[i].transform.position = GetScreenPositionFromTileTable(tileVec);
                    i++;
                    break;
            }
        }
    }
}