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

    void Start()
    {
		_npcController = GameManager.Instance._currentNPC;
		npcRenderer.material = _npcController.spriteMaterial;
        DoNewDialogueLoop();

	}

    public void ChooseDialogueOption(int anOptionID)
    {

    }

    public void InitDialogue(BeigeNPC npcController)
    {
        _npcController = npcController;
        textAnimator.SetAndStartAnimation(npcController.GetNextDialogueString());
	}

    private void DoNewDialogueLoop()
    {
		textAnimator.SetAndStartAnimation(_npcController.GetNextDialogueString());
        string[] lineoptions = _npcController.GetDialogueOptions();
        options[0].gameObject.SetActive(false);
        options[1].gameObject.SetActive(false);
        options[2].gameObject.SetActive(false);
        options[3].gameObject.SetActive(false);


        for (int i = 0; i < lineoptions.Length; i++)
        {
            options[i].gameObject.SetActive(true);
            options[i].TextForOption.SetText(lineoptions[i]);
        }
	}

}
