using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    int rows;
    int columns;
    Dictionary<GameObject, Vector2Int> moveObjPositionOnStage = new();
    FloorLayer floorLayer;
    MutableBoardState boardState;
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
        byte[] bytes = stageFile.bytes;
        Utf8JsonReader reader = new(new System.ReadOnlySpan<byte>(bytes));
        StageDecoder.Result result = StageDecoder.Decode(JsonNode.Parse(ref reader).AsObject());
        floorLayer = result.floorLayer;
        boardState = result.boardState;
        rows = result.size.y;
        columns = result.size.x;
    }

    public void CreateStage()
    {
        boardRenderer.rows = rows;
        boardRenderer.columns = columns;
        boardRenderer.CreateStage(floorLayer, boardState);
    }
    public bool IsWall(TileVec position)
    {
        return floorLayer.GetTile(position) == FloorLayerTile.WALL;
    }
    public bool IsBlock(TileVec position)
    {
        if (!boardState.HasObject(position)) return false;
        return boardState.GetObject(position).type == BoardObjectTypes.BLOCK;
    }
    public bool IsAllClear()
    {
        int clearCount = 0;
        int blockCount = 0;
        foreach (var (pos, boardObject) in boardState)
        {
            if (boardObject.type == BoardObjectTypes.BLOCK)
            {
                blockCount++;
                if (floorLayer.GetTile(pos) == FloorLayerTile.POINT)
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
                if (boardObject.type == BoardObjectTypes.PLAYER) return cache;
            }
        }
        TileVec? playerPos = null;
        foreach (var (tileVec, boardObject) in boardState)
        {
            if (boardObject.type == BoardObjectTypes.PLAYER)
            {
                playerPos = tileVec;
                break;
            }
        }
        playerPosCache = playerPos;
        return playerPos;
    }
    public void Undo()
    {
        if (history.TryPop(out BoardState result))
        {
            boardState.Overwrite(result);
            boardRenderer.UpdateStage(floorLayer, boardState);
        }
    }
    public bool PlayerMove(TileDirection direction)
    {
        TileVec? playerPos = GetPlayerPos();
        if (playerPos is not null)
        {
            TileVec pos = (TileVec)playerPos;
            BoardState memento = boardState.ToImmutable();
            TileVec nextPlayerPos = pos - direction;
            if (IsWall(nextPlayerPos)) return false;
            if (IsBlock(nextPlayerPos))
            {
                TileVec nextBlockPos = nextPlayerPos - direction;
                if (IsWall(nextBlockPos)) return false;
                boardState.MoveObject(nextPlayerPos, nextBlockPos);
            }
            playerPosCache = nextPlayerPos;
            boardState.MoveObject(pos, nextPlayerPos);
            boardRenderer.UpdateStage(floorLayer, boardState);
            history.Push(memento);
            return true;
        }
        return false;
    }
}
