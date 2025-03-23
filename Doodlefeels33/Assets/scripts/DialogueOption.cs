using TMPro;
using UnityEngine;

public class DialogueOption : MonoBehaviour
{
    [SerializeField]
    int myOptionID = -1;
    [SerializeField]
    DialogueManager dialogueManager;
    [SerializeField]
    AudioSource click_sfx;

	public TMP_Text TextForOption;

	public void OnClickedOption()
    {
        click_sfx.Play();
        dialogueManager.ChooseDialogueOption(myOptionID);
    }

}
