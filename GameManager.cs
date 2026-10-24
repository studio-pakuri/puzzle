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
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PlayerMove();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PlayerMove();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlayerMove();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlayerMove();
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
