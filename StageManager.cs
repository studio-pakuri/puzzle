using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    float tileSize;
    Vector2 centerPosition;
    Dictionary<GameObject, Vector2Int> moveObjPositionOnStage = new();
    FloorLayer floorLayer;
    MutableBoardState boardState = new();
    Stack<BoardState> history = new();
    PlayerManager playerManager;
    [SerializeField] TextAsset stageFile;
    [SerializeField] GameObject[] prefabs;

    public enum StageType
    {
        WALL,
        GROUND,
        BLOCK_POINT,
        BLOCK,
        PLAYER,
        BLOCK_ON_POINT,
        PLAYER_ON_POINT
    }
    StageType[,] stageTable;

    public void LoadStageData()
    {
        string[] lines = stageFile.text.Split(new[] { '\n', '\r' });
        int rows = lines.Length;
        int columns = lines[0].Split(new[] { ',' }).Length;
        Dictionary<TileVec, FloorLayer.FloorLayerTile> tiles = new();
        stageTable = new StageType[rows, columns];
        for (int x = 0; x < rows; x++)
        {
            string[] values = lines[x].Split(new[] { ',' });
            for (int y = 0; y < columns; y++)
            {
                StageType stageType = (StageType)int.Parse(values[y]);
                stageTable[x, y] = stageType;
                TileVec pos = new TileVec(y, x);
                tiles[pos] = stageType switch
                {
                    StageType.WALL => FloorLayer.FloorLayerTile.WALL,
                    StageType.BLOCK_POINT => FloorLayer.FloorLayerTile.POINT,
                    _ => FloorLayer.FloorLayerTile.FLOOR,
                };
                switch (stageType)
                {
                    case StageType.BLOCK:
                        boardState.SetObject(new BoardObject(BoardObjectType.BLOCK), pos);
                        break;
                    case StageType.PLAYER:
                        boardState.SetObject(new BoardObject(BoardObjectType.PLAYER), pos);
                        break;
                }
            }
        }
        floorLayer = new FloorLayer(tiles);
    }

    public void CreateStage()
    {
        tileSize = prefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
        centerPosition.x = stageTable.GetLength(0) / 2 * tileSize;
        centerPosition.y = stageTable.GetLength(1) / 2 * tileSize;
        for (int x = 0; x < stageTable.GetLength(0); x++)
        {
            for (int y = 0; y < stageTable.GetLength(1); y++)
            {
                Vector2Int positionInt = new(y, x);
                Vector2 position = GetScreenPositionFromTileTable(positionInt);
                TileVec tileVec = new(y, x);
                FloorLayer.FloorLayerTile? tile = floorLayer.GetTile(tileVec);
                if (tile is not null){
                    GameObject groundObj = Instantiate(prefabs[tile switch
                    {
                        FloorLayer.FloorLayerTile.WALL => (int)StageType.WALL,
                        FloorLayer.FloorLayerTile.FLOOR => (int)StageType.GROUND,
                        FloorLayer.FloorLayerTile.POINT => (int)StageType.BLOCK_POINT
                    }]);
                    groundObj.transform.position = position;
                }
                if (boardState.HasObject(tileVec))
                {
                    BoardObject boardObject = boardState.GetObject(tileVec);
                    StageType stageType = boardObject.type switch
                    {
                        BoardObjectType.PLAYER => StageType.PLAYER,
                        BoardObjectType.BLOCK => StageType.BLOCK
                    };
                    GameObject obj = Instantiate(prefabs[(int)stageType]);
                    obj.transform.position = position;
                    if (stageType == StageType.PLAYER)
                    {
                        playerManager = obj.GetComponent<PlayerManager>();
                        moveObjPositionOnStage.Add(obj, positionInt);
                    }
                    else if (stageType == StageType.BLOCK)
                    {
                        moveObjPositionOnStage.Add(obj, positionInt);
                    }
                }
            }
        }
    }
    public PlayerManager GetPlayerManager()
    {
        return playerManager;
    }
    public Vector2 GetScreenPositionFromTileTable(Vector2Int position)
    {
        return new Vector2(position.x * tileSize - centerPosition.x, -position.y * tileSize + centerPosition.y);
    }
    public void SetObjPositionOnStage(GameObject obj, Vector2Int pos)
    {
        moveObjPositionOnStage[obj] = pos;
    }
    public Vector2Int GetObjPosition(GameObject obj)
    {
        return moveObjPositionOnStage[obj];
    }
    public bool IsWall(Vector2Int position)
    {
        return floorLayer.GetTile(TileVecHelper.vector2IntToTileVec(position)) == FloorLayer.FloorLayerTile.WALL;
    }
    public bool IsBlock(Vector2Int position)
    {
        TileVec tileVec = TileVecHelper.vector2IntToTileVec(position);
        if (!boardState.HasObject(tileVec)) return false;
        return boardState.GetObject(tileVec).type == BoardObjectType.BLOCK;
    }
    public GameObject GetObjectByPosition(Vector2 position)
    {
        foreach (var item in moveObjPositionOnStage)
        {
            if (item.Value == position)
            {
                return item.Key;
            }
        }
        return null;
    }
    public void UpdateObjectPosition(Vector2Int nowPosition, Vector2Int nextPosition, StageType stageType)
    {
        boardState.MoveObject(
            TileVecHelper.vector2IntToTileVec(nowPosition),
            TileVecHelper.vector2IntToTileVec(nextPosition)
            );
        GameObject targetBlock = GetObjectByPosition(nowPosition);
        targetBlock.transform.position = GetScreenPositionFromTileTable(nextPosition);

        moveObjPositionOnStage[targetBlock] = nextPosition;
        if (stageTable[nextPosition.y, nextPosition.x] == StageType.BLOCK_POINT)
        {
            stageTable[nextPosition.y, nextPosition.x] = StageType.BLOCK_ON_POINT;
        }
        else
        {
            stageTable[nextPosition.y, nextPosition.x] = stageType;
        }
    }

    public void UpdateStageTableForPlayer(Vector2Int nowPosition, Vector2Int nextPosition)
    {
        boardState.RemoveObject(TileVecHelper.vector2IntToTileVec(nowPosition), out _);
        boardState.SetObject(new BoardObject(BoardObjectType.PLAYER), TileVecHelper.vector2IntToTileVec(nextPosition));
        if (
            stageTable[nextPosition.y, nextPosition.x] == StageType.BLOCK_ON_POINT
            || stageTable[nextPosition.y, nextPosition.x] == StageType.BLOCK_POINT
            )
        {
            stageTable[nextPosition.y, nextPosition.x] = StageType.PLAYER_ON_POINT;
        }
        else
        {
            stageTable[nextPosition.y, nextPosition.x] = StageType.PLAYER;
        }

        if (stageTable[nowPosition.y, nowPosition.x] == StageType.PLAYER_ON_POINT)
        {
            stageTable[nowPosition.y, nowPosition.x] = StageType.BLOCK_POINT;
        }
        else
        {
            stageTable[nowPosition.y, nowPosition.x] = StageType.GROUND;
        }
    }
    public bool IsAllClear()
    {
        int clearCount = 0;
        int blockCount = 0;
        foreach (var (pos, boardObject) in boardState)
        {
            if (boardObject.type == BoardObjectType.BLOCK)
            {
                blockCount++;
                if (floorLayer.GetTile(pos) == FloorLayer.FloorLayerTile.POINT)
                {
                    clearCount++;
                }
            }
        }
        return blockCount == clearCount;
    }
}
