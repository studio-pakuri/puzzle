using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    PlayerManager playerManager;
    [SerializeField] StageManager stageManager;
    void Start()
    {
        stageManager.LoadStageData();
        stageManager.CreateStage();
        playerManager = stageManager.GetPlayerManager();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PlayerMove(Vector2Int.up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PlayerMove(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlayerMove(Vector2Int.left);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlayerMove(Vector2Int.right);
        }
        CheckClear();
    }

    void PlayerMove(Vector2Int offset)
    {
        Vector2Int currentPlayerPosition = stageManager.GetObjPosition(playerManager.gameObject);
        Vector2Int nextPlayerPosition = currentPlayerPosition - offset;
        if (stageManager.IsWall(nextPlayerPosition)) return;
        if (stageManager.IsBlock(nextPlayerPosition))
        {
            Vector2Int nextBlockPosition = nextPlayerPosition - offset;
            if (stageManager.IsWall(nextBlockPosition) || stageManager.IsBlock(nextBlockPosition)) return;
            stageManager.UpdateObjectPosition(nextPlayerPosition, nextBlockPosition, StageManager.StageType.BLOCK);
        }
        stageManager.UpdateStageTableForPlayer(currentPlayerPosition, nextPlayerPosition);
        playerManager.Move(stageManager.GetScreenPositionFromTileTable(nextPlayerPosition));
        stageManager.SetObjPositionOnStage(playerManager.gameObject, nextPlayerPosition);
    }

    void CheckClear()
    {
        if (stageManager.IsAllClear())
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
    }
}
