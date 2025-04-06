using UnityEngine;
using UnityEngine.SceneManagement;

public class main_menu : MonoBehaviour
{
    [SerializeField]
    GameObject title_screen, black_screen, tutorial1_screen, tutorial2_screen;
    public AudioSource click_sfx;

    public void StartTutorial()
    {
        click_sfx.Play();
        title_screen.SetActive(false);
        black_screen.SetActive(true);
    }

    public void NextStep()
    {
        click_sfx.Play();
        tutorial1_screen.SetActive(false);
        tutorial2_screen.SetActive(true);
    }

    public void NewGame()
    {
        click_sfx.Play();
        SceneManager.LoadScene("MainGame");
    }

    public void QuitGame()
    {
        click_sfx.Play();
        Application.Quit();
    }
}
