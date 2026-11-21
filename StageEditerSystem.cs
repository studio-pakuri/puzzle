using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StageEditerSystem : MonoBehaviour
{
    public BoardRenderer boardRenderer;
    int mode = 0;
    readonly Dictionary<TileVec, FloorLayerTile> floor = new();
    readonly MutableBoardState board = new();
    void Start()
    {
        boardRenderer.CreateStage(new DictionaryFloorViewWrapper(floor), board);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
        {
            Vector2 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TileVec tileVec = boardRenderer.tileVecConverter.TileVecFromScreenPosition(vector);
            switch (mode)
            {
                case 0:
                    floor.Remove(tileVec);
                    break;
                case 1:
                    floor[tileVec] = FloorLayerTile.WALL;
                    break;
                case 2:
                    floor[tileVec] = FloorLayerTile.FLOOR;
                    break;
                case 3:
                    floor[tileVec] = FloorLayerTile.POINT;
                    break;
            }
            boardRenderer.UpdateStage(new DictionaryFloorViewWrapper(floor), board);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            mode += 1;
            mode %= 4;
        }
    }
}
