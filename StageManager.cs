using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    int rows;
    int columns;
    float tileSize;
    Vector2 centerPosition;
    Dictionary<GameObject, Vector2Int> moveObjPositionOnStage = new();
    FloorLayer floorLayer;
    MutableBoardState boardState = new();
    Stack<BoardState> history = new();
    PlayerManager playerManager;
    [SerializeField] TextAsset stageFile;
    [SerializeField] BoardRenderer boardRenderer;

    public enum StageType
    {
        WALL,
        GROUND,
        BLOCK_POINT,
        BLOCK,
        PLAYER
    }

    public void LoadStageData()
    {
        string[] lines = stageFile.text.Split(new[] { '\n', '\r' });
        rows = lines.Length;
        columns = lines[0].Split(new[] { ',' }).Length;
        Dictionary<TileVec, FloorLayer.FloorLayerTile> tiles = new();
        for (int x = 0; x < rows; x++)
        {
            string[] values = lines[x].Split(new[] { ',' });
            for (int y = 0; y < columns; y++)
            {
                StageType stageType = (StageType)int.Parse(values[y]);
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
        boardRenderer.rows = rows;
        boardRenderer.columns = columns;
        boardRenderer.CreateStage(floorLayer, boardState);
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
    }

    public void UpdateStageTableForPlayer(Vector2Int nowPosition, Vector2Int nextPosition)
    {
        boardState.RemoveObject(TileVecHelper.vector2IntToTileVec(nowPosition), out _);
        boardState.SetObject(new BoardObject(BoardObjectType.PLAYER), TileVecHelper.vector2IntToTileVec(nextPosition));
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
    

    void PlayerMove(TileDirection direction)
    {
        TileVec? playerPos = null;
        foreach (var (tileVec, boardObject) in boardState)
        {
            if (boardObject.type == BoardObjectType.PLAYER)
            {
                playerPos = tileVec;
                break;
            }
        }
        if (playerPos is not null)
        {
            TileVec pos = (TileVec)playerPos;
            if (IsWall(pos + (TileVec)direction)) return;
        }
    }
}
