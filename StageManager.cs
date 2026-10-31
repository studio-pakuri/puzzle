using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    int rows;
    int columns;
    Dictionary<GameObject, Vector2Int> moveObjPositionOnStage = new();
    FloorLayer floorLayer;
    MutableBoardState boardState = new();
    Stack<BoardState> history = new();
    [SerializeField] TextAsset stageFile;
    [SerializeField] BoardRenderer boardRenderer;
    TileVec? playerPosCache;

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
    public bool IsWall(TileVec position)
    {
        return floorLayer.GetTile(position) == FloorLayer.FloorLayerTile.WALL;
    }
    public bool IsBlock(TileVec position)
    {
        if (!boardState.HasObject(position)) return false;
        return boardState.GetObject(position).type == BoardObjectType.BLOCK;
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
    
    public TileVec? GetPlayerPos()
    {
        if (playerPosCache is not null)
        {
            TileVec cache = (TileVec)playerPosCache;
            
            if (boardState.TryGetObject(cache, out BoardObject boardObject))
            {
                if (boardObject.type == BoardObjectType.PLAYER) return cache;
            }
        }
        TileVec? playerPos = null;
        foreach (var (tileVec, boardObject) in boardState)
        {
            if (boardObject.type == BoardObjectType.PLAYER)
            {
                playerPos = tileVec;
                break;
            }
        }
        playerPosCache = playerPos;
        return playerPos;
    }
    public void PlayerMove(TileDirection direction)
    {
        TileVec? playerPos = GetPlayerPos();
        if (playerPos is not null)
        {
            TileVec pos = (TileVec)playerPos;
            TileVec nextPlayerPos = pos - direction;
            if (IsWall(nextPlayerPos)) return;
            if (IsBlock(nextPlayerPos))
            {
                TileVec nextBlockPos = nextPlayerPos - direction;
                if (IsWall(nextBlockPos)) return;
                boardState.MoveObject(nextPlayerPos, nextBlockPos);
            }
            playerPosCache = nextPlayerPos;
            boardState.MoveObject(pos, nextPlayerPos);
            boardRenderer.UpdateStage(boardState);
        }
    }
}
