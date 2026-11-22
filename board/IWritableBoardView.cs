public interface IWritableBoardView : IBoardView
{
    public void SetObject(BoardObject boardObject, TileVec pos);
    public bool RemoveObject(TileVec pos, out BoardObject boardObject);
}