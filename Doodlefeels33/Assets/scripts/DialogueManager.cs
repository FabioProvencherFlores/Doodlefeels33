using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;



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

    [SerializeField]
    AudioSource chain_click_sfx;

	private void Update()
	{
		if (textAnimator.GetIsAnimating())
		{
			if (Input.GetMouseButtonDown(0))
            {
                textAnimator.ForceFullText();
            }
		}
	}

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
        DoNewDialogueLoop();
	}

    public void ClickedJail()
    {
        if (textAnimator.GetIsAnimating()) return;
        _npcController.ProcessDialogueOption(4);
        chain_click_sfx.Play();
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

        StartCoroutine(WaitTextAndPrintOptions());
	}

	IEnumerator WaitTextAndPrintOptions()
	{
		while (textAnimator.GetIsAnimating())
		{
			yield return new WaitForSeconds(0.15f);
		}

        List<string> lineoptions = _npcController.GetDialogueOptions();
		if (lineoptions.Count < 4 && _npcController.IsGoodbyeADefaultOption())
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
