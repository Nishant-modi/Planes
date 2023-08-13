using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject gameWinUI;
    bool gameIsOver;

    // Start is called before the first frame update
    void Start()
    {
        Guard.OnGuardHasSpottedPlayer += GameOverUI;
        FindObjectOfType<ThirdPresonMovement>().OnReachedEndOfLevel += GameWinUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
        }
    }

    public void GameWinUI()
    {
        GameDone(gameWinUI);
    }

    public void GameOverUI()
    {
        GameDone(gameOverUI);
    }

    public void GameDone(GameObject UI)
    {
        UI.SetActive(true);
        gameIsOver = true;
        Guard.OnGuardHasSpottedPlayer -= GameOverUI;
        FindObjectOfType<ThirdPresonMovement>().OnReachedEndOfLevel -= GameWinUI;
    }
}
