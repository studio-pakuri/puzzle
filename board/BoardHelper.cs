
public static class BoardHelper
{
    public static bool TryGetObject(this IBoardView boardView, TileVec pos, out BoardObject boardObject)
    {
        if (boardView.HasObject(pos))
        {
            boardObject = boardView.GetObject(pos);
            return true;
        }
        else
        {
            boardObject = null;
            return false;
        }
    }
    public static bool MoveObject(this IWritableBoardView boardView, TileVec from, TileVec to)
    {
        if (boardView.HasObject(to)) return false;
        if (boardView.RemoveObject(from, out BoardObject boardObject))
        {
            boardView.SetObject(boardObject, to);
            return true;
        }
        return false;
    }
}