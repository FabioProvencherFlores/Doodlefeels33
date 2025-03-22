using TMPro;
using UnityEngine;

public class DialogueOption : MonoBehaviour
{
    [SerializeField]
    int myOptionID = -1;
    [SerializeField]
    DialogueManager dialogueManager;

	public TMP_Text TextForOption;

	public void OnClickedOption()
    {
        dialogueManager.ChooseDialogueOption(myOptionID);
    }

}
