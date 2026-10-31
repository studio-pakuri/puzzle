
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
}