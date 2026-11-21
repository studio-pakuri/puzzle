using System.Collections;
using System.Collections.Generic;

public class FloorLayer : IFloorView
{
    readonly Dictionary<TileVec, FloorLayerTile> tiles;
    readonly HashSet<TileVec> points;

    public FloorLayer(IEnumerable<KeyValuePair<TileVec, FloorLayerTile>> keyValuePairs)
    {
        tiles = new Dictionary<TileVec, FloorLayerTile>(keyValuePairs);
        points = new();
        foreach (var (tileVec, floorLayerTile) in keyValuePairs)
        {
            if (floorLayerTile == FloorLayerTile.POINT)
            {
                points.Add(tileVec);
            }
        }
    }

    public IEnumerable<TileVec> Points => points;

    public IEnumerator<KeyValuePair<TileVec, FloorLayerTile>> GetEnumerator() => tiles.GetEnumerator();

    public FloorLayerTile? GetTile(TileVec pos)
    {
        if (tiles.ContainsKey(pos)){
            return tiles[pos];
        }
        else
        {
            return null;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
}
