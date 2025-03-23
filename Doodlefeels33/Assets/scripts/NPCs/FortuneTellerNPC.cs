using System;
using UnityEngine;

public class FortuneTellerNPC : BeigeNPC, IDialogue
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
				currentline = "I miss the stars more than anything. What living is there for us if we can't look at the sky that saw us grow.";
				break;
			case SITUATION.PlayerAskedToGoToJail:
				removeGoodbye = true;
				if (myData.playerHasAskedForJail)
				{
					currentline = "This again?";
				}
				else
				{
					currentline = "I don't see a future where I'll be emprisoned by my own kind. The Eye will not tolerate it.";
				}
				dialogueOptions.Add("Nevermind.");
				dialogueOptions.Add("We need to do this. Please go in.");
				break;
			case SITUATION.BackedDownFromJailRequest:
				currentline = "I see.";
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
		return 8;
	}

	public void InitNewDialogue()
	{
		currentContext = GetInitialContext();
		nextContext = currentContext;
		myData.Reset();
	}
}
