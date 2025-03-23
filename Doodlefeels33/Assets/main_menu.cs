using UnityEngine;
using UnityEngine.SceneManagement;

public class main_menu : MonoBehaviour
{
    [SerializeField]
    GameObject title_screen, black_screen, tutorial1_screen, tutorial2_screen;

    public void StartTutorial()
    {
        title_screen.SetActive(false);
        black_screen.SetActive(true);
    }

    public void NextStep()
    {
        tutorial1_screen.SetActive(false);
        tutorial2_screen.SetActive(true);
    }




    public void NewGame()
    {
        SceneManager.LoadScene("MainGame");
    }
}
