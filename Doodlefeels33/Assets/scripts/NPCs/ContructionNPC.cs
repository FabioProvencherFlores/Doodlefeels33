using System;
using UnityEngine;

public class ContructionNPC : BeigeNPC, IDialogue
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
	bool _saidFirstInfo = false;
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
				if (!_saidFirstInfo)
				{
					currentline = "I just got done with the windows, and that fucking sun is already peering through. We won't last long at this rate...";
					_saidFirstInfo = true;
				}
				else if (GameManager.Instance.IsMorning()) currentline = "Mornin'";
				else if (GameManager.Instance.IsEvening()) currentline = "Evenin'";
				else currentline = "Hey.";
			
				break;
			case SITUATION.PlayerAskedToGoToJail:
				removeGoodbye = true;
				currentline = "Really? Do you want me to tie the chains myself while you're at it? Piss off!";
				dialogueOptions.Add("Okay, stay here if you want.");
				dialogueOptions.Add("We all agreed, this is for the best. Please get in there.");
				break;
			case SITUATION.BackedDownFromJailRequest:
				currentline = "Fucking pussy.";
				break;
		}


		return currentline;
	}


	public void ProcessDialogueOption(int optionID)
	{
		switch (currentContext)
		{
			case SITUATION.PlayerAskedToGoToJail:
				if (optionID == 0) nextContext = SITUATION.BackedDownFromJailRequest;
				if (optionID == 1)
				{
					amMissing = true;
					amDead = true;
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
			myData.playerHasAskedForJail = true;
		}
	}

	public SITUATION GetInitialContext()
	{
		return SITUATION.NormalGreating;
	}

	public int GetNPCID()
	{
		return 9;
	}
	public void InitNewDialogue()
	{
		currentContext = GetInitialContext();
		nextContext = currentContext;
		myData.Reset();
	}
}
