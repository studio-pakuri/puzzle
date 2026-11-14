using System.Collections;
using System.Collections.Generic;

public readonly struct DictionaryFloorViewWrapper : IFloorView
{
    readonly IReadOnlyDictionary<TileVec, FloorLayerTile> dictionary;
    public DictionaryFloorViewWrapper(IReadOnlyDictionary<TileVec, FloorLayerTile> dictionary)
    {
        this.dictionary = dictionary;
    }
    public IEnumerator<KeyValuePair<TileVec, FloorLayerTile>> GetEnumerator() => dictionary.GetEnumerator();

    public IEnumerable<TileVec> Points => dictionary.Keys;

    public FloorLayerTile? GetTile(TileVec pos)
    {
        if (dictionary.TryGetValue(pos, out FloorLayerTile tile))
        {
            return tile;
        }
        else
        {
            return null;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}