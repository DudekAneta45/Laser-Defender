using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour {

    [SerializeField] float delayForSeconds = 1;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
        FindObjectOfType<GameSession>().ResetGame();
    }
    public void LoadGameOver()
    {
        StartCoroutine(DelayGameOver());
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(delayForSeconds);
        SceneManager.LoadScene("Game Over");
    }


}
