using System.Collections.Generic;

public interface IFloorView : IEnumerable<KeyValuePair<TileVec, FloorLayerTile>>
{
    IEnumerable<TileVec> Points { get; }

    FloorLayerTile? GetTile(TileVec pos);
}