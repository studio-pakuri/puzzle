using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] StageManager stageManager;
    void Start()
    {
        stageManager.LoadStageData();
        stageManager.CreateStage();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            stageManager.PlayerMove(TileDirection.UP);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            stageManager.PlayerMove(TileDirection.DOWN);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            stageManager.PlayerMove(TileDirection.LEFT);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            stageManager.PlayerMove(TileDirection.RIGHT);
        }
        CheckClear();
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
