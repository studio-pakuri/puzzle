using System.Collections;
using System.Collections.Generic;

public class BoardState: IEnumerable<KeyValuePair<TileVec, BoardObject>>
{
    readonly Dictionary<TileVec, BoardObject> objects;
    public BoardState(IEnumerable<KeyValuePair<TileVec, BoardObject>> keyValuePairs)
    {
        objects = new Dictionary<TileVec, BoardObject>(keyValuePairs);
    }

    public IEnumerator<KeyValuePair<TileVec, BoardObject>> GetEnumerator() => objects.GetEnumerator();

    public BoardObject GetObject(TileVec pos) => objects[pos];

    public bool HasObject(TileVec pos) => objects.ContainsKey(pos);

    public MutableBoardState ToMutable()
    {
        return new MutableBoardState(objects);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}