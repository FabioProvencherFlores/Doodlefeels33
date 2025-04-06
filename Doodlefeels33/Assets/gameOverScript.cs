using UnityEngine;
using UnityEngine.SceneManagement;

public class gameOverScript : MonoBehaviour
{
    [SerializeField]
    AudioSource click_sfx;
    public void goToMainMenu()
    {
        click_sfx.Play();
        SceneManager.LoadScene("TitleScreen");
    }

    public void goToNewGame()
    {
        click_sfx.Play();
        SceneManager.LoadScene("MainGame");
    }
}
