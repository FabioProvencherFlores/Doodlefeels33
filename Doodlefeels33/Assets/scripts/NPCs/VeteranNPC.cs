using System;
using UnityEngine;

public class VeteranNPC : BeigeNPC, IDialogue
{
	[Header("Dialogue Data")]
	[SerializeField]
	public Material customSprite;
	public Material spriteMaterial
	{
		get
		{
			return customSprite;
		}
	}

	public string GetNextDialogueString()
	{
		removeGoodbye = false;
		dialogueOptions.Clear();
		SITUATION previousSituation = currentContext;
		currentContext = nextContext;
		string currentline = "I SHOULD NOT SAY THIS, FAB MUST HAVE FORGOTTEN SOMETHING";


		switch (currentContext)
		{
			case SITUATION.NormalGreating:
				currentline = "Yes captain?";
				break;
			case SITUATION.PlayerAskedToGoToJail:
				removeGoodbye = true;
				currentline = "If you say so. I will await for further instructions.";
				dialogueOptions.Add("On second thought, stand guard with us.");
				dialogueOptions.Add("Thanks for your cooperation.");
				break;
			case SITUATION.BackedDownFromJailRequest:
				currentline = "As you say.";
				break;
		}


		return currentline;
	}

	public void ProcessDialogueOption(int optionID)
	{
		switch (currentContext)
		{
			case SITUATION.PlayerAskedToGoToJail:
				myData.playerHasAskedForJail = true;
				if (optionID == 0) nextContext = SITUATION.BackedDownFromJailRequest;
				if (optionID == 1)
				{
					GameManager.Instance.PutCurrentNPCInJail();
					GameManager.Instance.GoToGym();
					return;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.NormalGreating:
			case SITUATION.BackedDownFromJailRequest:
			case SITUATION.PassiveChecks:
			default:
				if (optionID == 3)
				{
					GameManager.Instance.GoToGym();
				}
				break;
		}

		// 4 is jail
		if (optionID == 4)
		{
			nextContext = SITUATION.PlayerAskedToGoToJail;
			myData.playerWantsToJailMe = true;
		}
	}

	public SITUATION GetInitialContext()
	{
		return SITUATION.NormalGreating;
	}


	public int GetNPCID()
	{
		return 6;
	}
	public void InitNewDialogue()
	{
		currentContext = GetInitialContext();
		nextContext = currentContext;
		myData.Reset();
	}
}
