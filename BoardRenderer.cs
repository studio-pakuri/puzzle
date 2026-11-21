using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardRenderer : MonoBehaviour
{
    public int rows;
    public int columns;
    public TileVecConverter tileVecConverter;
    List<GameObject> blocks = new();
    GameObject player;
    [SerializeField] GameObject[] prefabs;
    readonly Dictionary<TileVec, FloorLayerTile> past = new();
    readonly Dictionary<TileVec, GameObject> tileObjects = new();

    public IEnumerable<TileVec> Difference(IFloorView floorLayer)
    {
        IFloorView view = new DictionaryFloorViewWrapper(past);
        return Enumerable.Union(floorLayer.Select(pair => pair.Key), view.Select(pair => pair.Key))
        .Where(vec => floorLayer.GetTile(vec) != view.GetTile(vec));
    }

    public void CreateStage(IFloorView floorLayer, IBoardView boardView)
    {
        tileVecConverter = new(prefabs[0].GetComponent<SpriteRenderer>().bounds.size.x);
        player = Instantiate(prefabs[4]);
        UpdateStage(floorLayer, boardView);
    }

    public void UpdateStage(IFloorView floorLayer, IBoardView boardView)
    {
        foreach (TileVec tileVec in Difference(floorLayer))
        {
            // 座標の位置にタイルを配置
            Vector2 position = tileVecConverter.ScreenPositionFromTileVec(tileVec);
            if (tileObjects.TryGetValue(tileVec, out GameObject tileObject))
            {
                Destroy(tileObject);
            }
            FloorLayerTile? tile = floorLayer.GetTile(tileVec);
            if (tile is not null)
            {
                GameObject gameObject = Instantiate(prefabs[(int)tile]);
                gameObject.transform.position = position;
                tileObjects[tileVec] = gameObject;
            }
            else
            {
                tileObjects.Remove(tileVec);
            }
        }
        past.Clear();
        foreach (var (tileVec, tile) in floorLayer)
        {
            past[tileVec] = tile;
        }
        int blockIndex = 0;
        foreach (var (tileVec, boardObject) in boardView)
        {
            // プレイヤーオブジェクトが存在しない時への対応が必要
            if (boardObject.type == BoardObjectTypes.PLAYER)
            {
                // プレイヤーオブジェクトを必要な位置に移動
                player.transform.position = tileVecConverter.ScreenPositionFromTileVec(tileVec);
            }
            else if (boardObject.type == BoardObjectTypes.BLOCK)
            {
                // ブロックオブジェクトを必要な位置に移動
                if (blocks.Count <= blockIndex)
                {
                    GameObject gameObject = Instantiate(prefabs[3]);
                    blocks.Add(gameObject);
                }
                blocks[blockIndex].SetActive(true);
                blocks[blockIndex].transform.position = tileVecConverter.ScreenPositionFromTileVec(tileVec);
                blockIndex++;
            }
        }
        for (int i = blockIndex; i < blocks.Count; i++)
        {
            blocks[i].SetActive(false);
        }
    }
}