using UnityEngine;
using UnityEngine.SceneManagement;

public class main_menu : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("MainGame");
    }
}
