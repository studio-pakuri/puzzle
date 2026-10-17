using System.Collections;
using System.Collections.Generic;


public class MutableBoardState: IEnumerable<KeyValuePair<TileVec, BoardObject>>
{
    readonly Dictionary<TileVec, BoardObject> objects;

    public MutableBoardState()
    {
        objects = new Dictionary<TileVec, BoardObject>();
    }
    public MutableBoardState(IEnumerable<KeyValuePair<TileVec, BoardObject>> keyValuePairs)
    {
        objects = new Dictionary<TileVec, BoardObject>(keyValuePairs);
    }

    public BoardObject GetObject(TileVec pos) => objects[pos];

    public bool HasObject(TileVec pos) => objects.ContainsKey(pos);

    public void SetObject(BoardObject boardObject, TileVec pos)
    {
        objects[pos] = boardObject;
    }

    public bool RemoveObject(TileVec pos, out BoardObject boardObject) => objects.Remove(pos, out boardObject);

    public bool MoveObject(TileVec from, TileVec to)
    {
        if (objects.ContainsKey(to)) return false;
        if (objects.Remove(from, out BoardObject boardObject))
        {
            objects[to] = boardObject;
            return true;
        }
        return false;
    }

    public void Clear()
    {
        objects.Clear();
    }

    public void Overwrite(BoardState boardState)
    {
        objects.Clear();
        foreach (var (pos, boardObject) in boardState)
        {
            objects[pos] = boardObject;
        }
    }

    public BoardState ToImmutable()
    {
        return new BoardState(objects);
    }

    public IEnumerator<KeyValuePair<TileVec, BoardObject>> GetEnumerator() => objects.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}