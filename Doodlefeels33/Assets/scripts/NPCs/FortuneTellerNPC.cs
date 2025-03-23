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
			case SITUATION.EscapeQuest:
				removeGoodbye = true;
				currentline = "I forsee a lot of Death outside. The Solar Eye is upon us, waiting. Are you sure you want us to leave?";
				dialogueOptions.Add("I trust your visions. Let's stay.");
				dialogueOptions.Add("Yes, we need to leave. Now.");
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
			case SITUATION.EscapeQuest:
				_playerAskedToLeave = true;
				if (optionID == 0)
				{
					nextContext = SITUATION.NormalGreating;
				}
				else if (optionID == 1)
				{
					amReadyToLeave = true;
					GameManager.Instance.GoToGym();
				}
				break;
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
	bool _playerAskedToLeave = false;
	public SITUATION GetInitialContext()
	{
		if (GameManager.Instance.npcsPrepareToLeave && !_playerAskedToLeave)
			return SITUATION.EscapeQuest;
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
