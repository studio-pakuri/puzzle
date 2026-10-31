using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
public static class StageDecoder
{
    public static Result Decode(JsonObject jsonObject)
    {
        Dictionary<TileVec, FloorLayer.FloorLayerTile> tiles = new();
        MutableBoardState boardState = new();
        JsonObject tileNode = jsonObject["tiles"].AsObject();
        JsonArray boardStateNode = jsonObject["objects"].AsArray();
        int sizeX = tileNode["x"].GetValue<int>();
        int sizeY = tileNode["y"].GetValue<int>();
        JsonArray tileArray = tileNode["array"].AsArray();
        if (tileArray.Count != sizeX * sizeY) throw new FormatException();
        int i = 0;
        foreach (var node in tileArray){
            int id = node.GetValue<int>();
            TileVec pos = new(i % sizeX, i / sizeX);
            tiles[pos] = (FloorLayer.FloorLayerTile)id;
            i++;
        }

        foreach (var node in boardStateNode)
        {
            int x = node["x"].GetValue<int>();
            int y = node["y"].GetValue<int>();
            int id = node["type"].GetValue<int>();
            TileVec pos = new(x, y);
            boardState.SetObject(new BoardObject((BoardObjectType)id), pos);
        }

        return new Result(new FloorLayer(tiles), boardState);
    }
    public readonly struct Result
    {
        public readonly FloorLayer floorLayer;
        public readonly MutableBoardState boardState;

        public Result(FloorLayer floorLayer, MutableBoardState boardState)
        {
            this.floorLayer = floorLayer;
            this.boardState = boardState;
        }
    }
}