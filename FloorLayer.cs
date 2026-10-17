using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLayer : IEnumerable<KeyValuePair<TileVec, FloorLayer.FloorLayerTile>>
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

    public IEnumerable<TileVec> GetPoints() => new Points(this);

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

    readonly struct Points : IEnumerable<TileVec>
    {
        readonly FloorLayer parent;
        public Points(FloorLayer parent)
        {
            this.parent = parent;
        }
        public IEnumerator<TileVec> GetEnumerator() => parent.points.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public enum FloorLayerTile
    {
        WALL,
        FLOOR,
        POINT
    }
    
}
