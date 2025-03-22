using UnityEngine;



public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    TextAnimator textAnimator;

    [SerializeField]
    MeshRenderer npcRenderer;

    private IDialogue _npcController;
    [SerializeField]
    DialogueOption[] options;

    public bool jailIsAvailable = true;


    public void ChooseDialogueOption(int anOptionID)
    {
        if (anOptionID < 0)
        {
            Debug.LogError("Invalid dialogue choice", this);
        }
        
        _npcController.ProcessDialogueOption(anOptionID);
        DoNewDialogueLoop();
	}

    public void InitDialogue(IDialogue npcController)
    {
		_npcController = npcController;
		npcRenderer.material = _npcController.spriteMaterial;
		textAnimator.SetAndStartAnimation(npcController.GetNextDialogueString());
        DoNewDialogueLoop();
	}

    public void ClickedJail()
    {
        _npcController.ProcessDialogueOption(4);
        DoNewDialogueLoop();

	}

    private void DoNewDialogueLoop()
    {
		textAnimator.SetAndStartAnimation(_npcController.GetNextDialogueString());
        string[] lineoptions = _npcController.GetDialogueOptions();
        options[0].gameObject.SetActive(false);
        options[1].gameObject.SetActive(false);
        options[2].gameObject.SetActive(false);
        options[3].gameObject.SetActive(false);

        if (lineoptions.Length <  4 && _npcController.IsGoodbyeADefaultOption())
        {
            options[3].gameObject.SetActive(true);
            options[3].TextForOption.SetText("Goodbye.");

        }

        for (int i = 0; i < lineoptions.Length; i++)
        {
            options[i].gameObject.SetActive(true);
            options[i].TextForOption.SetText(lineoptions[i]);
        }
	}

}
