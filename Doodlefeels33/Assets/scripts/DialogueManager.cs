using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;



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
        List<string> lineoptions = _npcController.GetDialogueOptions();
        options[0].gameObject.SetActive(false);
        options[1].gameObject.SetActive(false);
        options[2].gameObject.SetActive(false);
        options[3].gameObject.SetActive(false);

		if ((lineoptions.Count > 3 && _npcController.IsGoodbyeADefaultOption()) ||(lineoptions.Count == 4 && !_npcController.IsGoodbyeADefaultOption()))
		{
            Debug.LogError("too many dialogue options in the line " + _npcController.GetNextDialogueString());

		}

		if (lineoptions.Count <  4 && _npcController.IsGoodbyeADefaultOption())
        {
            options[3].gameObject.SetActive(true);
            options[3].TextForOption.SetText("Goodbye.");

        }

        for (int i = 0; i < lineoptions.Count; i++)
        {
            options[i].gameObject.SetActive(true);
            options[i].TextForOption.SetText(lineoptions[i]);
        }
	}

}
