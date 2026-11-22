using System.Collections;
using System.Collections.Generic;

public class DifferenceTrackingBoardDecorator : IWritableBoardView
{
    private readonly IWritableBoardView parent;
    private readonly Dictionary<TileVec, BoardObject> difference = new();
    public DifferenceTrackingBoardDecorator(IWritableBoardView parent)
    {
        this.parent = parent;
    }
    public IEnumerator<KeyValuePair<TileVec, BoardObject>> GetEnumerator() => parent.GetEnumerator();

    public BoardObject GetObject(TileVec pos) => parent.GetObject(pos);

    public bool HasObject(TileVec pos) => parent.HasObject(pos);

    public bool RemoveObject(TileVec pos, out BoardObject boardObject)
    {
        if (parent.RemoveObject(pos, out boardObject))
        {
            difference[pos] = boardObject;
            return true;
        }
        return false;
    }

    public void SetObject(BoardObject boardObject, TileVec pos)
    {
        if (parent.HasObject(pos))
        {
            difference[pos] = null;
        }
        else
        {
            difference[pos] = parent.GetObject(pos);
        }
        parent.SetObject(boardObject, pos);
    }

    IEnumerator IEnumerable.GetEnumerator() => parent.GetEnumerator();

    public IBoardPatch CreateBoardPatch() => new DifferenceBoardPatch(new Dictionary<TileVec, BoardObject>(difference));

    private class DifferenceBoardPatch : IBoardPatch
    {
        private IDictionary<TileVec, BoardObject> difference;
        public DifferenceBoardPatch(IDictionary<TileVec, BoardObject> difference)
        {
            this.difference = difference;
        }
        public bool Patch(IWritableBoardView boardView)
        {
            foreach ((TileVec pos, BoardObject boardObject) in difference)
            {
                if (boardObject is not null)
                {
                    boardView.SetObject(boardObject, pos);
                }
                else
                {
                    boardView.RemoveObject(pos, out _);
                }
            }
            return true;
        }
    }
}