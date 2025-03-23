using UnityEngine;
using UnityEngine.SceneManagement;
public class winScene_script : MonoBehaviour
{
    public void goToMainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
