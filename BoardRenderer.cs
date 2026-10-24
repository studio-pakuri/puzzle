using System.Collections.Generic;
using UnityEngine;

public class BoardRenderer: MonoBehaviour
{
    int rows;
    int columns;
    float tileSize;
    Vector2 centerPosition;
    List<GameObject> blocks;
    GameObject player;
    [SerializeField] GameObject[] prefabs;
    public Vector2 GetScreenPositionFromTileTable(Vector2Int position)
    {
        return new Vector2(position.x * tileSize - centerPosition.x, -position.y * tileSize + centerPosition.y);
    }

    public void CreateStage(FloorLayer floorLayer, IBoardView boardView)
    {
        
    }

    public void UpdateStage(IBoardView boardView)
    {
        foreach (var (tileVec, boardObject) in boardView)
        {
            
        }
    }
}