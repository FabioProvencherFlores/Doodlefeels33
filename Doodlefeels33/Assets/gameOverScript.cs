using UnityEngine;
using UnityEngine.SceneManagement;

public class gameOverScript : MonoBehaviour
{
    public void goToMainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void goToNewGame()
    {
        SceneManager.LoadScene("MainGame");
    }
}
