using UnityEngine;
using UnityEngine.SceneManagement;
public class winScene_script : MonoBehaviour
{
    [SerializeField]
    AudioSource click_sfx;
    public void goToMainMenu()
    {
        click_sfx.Play();
        SceneManager.LoadScene("TitleScreen");
    }
}
