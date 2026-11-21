using System.Collections;
using System.Collections.Generic;
using System.Linq;

public readonly struct DictionaryFloorViewWrapper : IFloorView
{
    readonly IReadOnlyDictionary<TileVec, FloorLayerTile> dictionary;
    public DictionaryFloorViewWrapper(IReadOnlyDictionary<TileVec, FloorLayerTile> dictionary)
    {
        this.dictionary = dictionary;
    }
    public IEnumerator<KeyValuePair<TileVec, FloorLayerTile>> GetEnumerator() => dictionary.GetEnumerator();

    public IEnumerable<TileVec> Points => dictionary.Where(pair => pair.Value == FloorLayerTile.POINT).Select(pair => pair.Key);

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