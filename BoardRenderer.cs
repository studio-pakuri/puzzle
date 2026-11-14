using System.Collections.Generic;
using UnityEngine;

public class BoardRenderer : MonoBehaviour
{
    public int rows;
    public int columns;
    float tileSize;
    List<GameObject> blocks = new();
    GameObject player;
    [SerializeField] GameObject[] prefabs;
    public Vector2 GetScreenPositionFromTileTable(TileVec position)
    {
        return new Vector2(position.x * tileSize, -position.y * tileSize);
    }

    public void CreateStage(IFloorView floorLayer, IBoardView boardView)
    {
        tileSize = prefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                // 座標の位置にタイルを配置
                TileVec tileVec = new(y, x);
                Vector2 position = GetScreenPositionFromTileTable(tileVec);
                FloorLayerTile? tile = floorLayer.GetTile(tileVec);
                if (tile is not null)
                {
                    GameObject groundObj = Instantiate(prefabs[tile switch
                    {
                        FloorLayerTile.WALL => (int)StageManager.StageType.WALL,
                        FloorLayerTile.FLOOR => (int)StageManager.StageType.GROUND,
                        FloorLayerTile.POINT => (int)StageManager.StageType.BLOCK_POINT
                    }]);
                    groundObj.transform.position = position;
                }

                // 座標の位置にブロックがあった場合はブロックオブジェクトを追加
                if (boardView.HasObject(tileVec))
                {
                    BoardObjectType type = boardView.GetObject(tileVec).type;
                    if (type == BoardObjectTypes.BLOCK)
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
            if (boardObject.type == BoardObjectTypes.PLAYER)
            {
                // プレイヤーオブジェクトを必要な位置に移動
                player.transform.position = GetScreenPositionFromTileTable(tileVec);
            }
            else if (boardObject.type == BoardObjectTypes.BLOCK)
            {
                // ブロックオブジェクトを必要な位置に移動
                blocks[i].transform.position = GetScreenPositionFromTileTable(tileVec);
                i++;
            }
        }
    }
}