using System.Collections;
using System.Collections.Generic;

public interface IBoardView: IEnumerable<KeyValuePair<TileVec, BoardObject>>
{
    public BoardObject GetObject(TileVec pos);
    public bool HasObject(TileVec pos);
    
    
}