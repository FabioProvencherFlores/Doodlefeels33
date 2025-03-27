using System;
using UnityEngine;

public class itNPC : BeigeNPC, IDialogue
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
	int _numberOfWorkInterruptions = 0;
	bool _gaveInfoAboutEscape = false;
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
				if (_numberOfWorkInterruptions == 0) currentline = "I heard noise over the radio but the battery is dead. Maybe I can some juice out of this. The transistor is busted... Ho, can I help you?";
				else if (_numberOfWorkInterruptions == 1) currentline = "I just need this part. Come on, just turn on. On last time?";
				else if (_numberOfWorkInterruptions == 2) currentline = "Batteries. I need batteries. Maybe that old hag got some left... Yes. Maybe I can... borrow... some. I need batteries. Ho, didn't see you there.";
				else if (_numberOfWorkInterruptions == 3) currentline = "Need power. Need light. Need power. Need more. Need power. Need. Power. More. Light.";
				else if (_numberOfWorkInterruptions == 4) currentline = "What?";
				if (GameManager.Instance.playerFoundBatteries)
				{
					dialogueOptions.Add("I got some batteries. Would that be of use?");
				}
				break;
			case SITUATION.PlayerAskedToGoToJail:
				removeGoodbye = true;
				currentline = "I won't be of any use in total darkness. Think about it, okay?";
				dialogueOptions.Add("You are right. Stay with us.");
				dialogueOptions.Add("You need to go inside, it won't be for long.");
				break;
			case SITUATION.EscapeQuest:
				if (!_gaveInfoAboutEscape)
				{
					currentline = "Hold on. I think it works, I'm receiving something... Something about a sage haven up north. Snow clouds protect people from the Solar Eye! We need to leave soon! 3 days and I'm out!";
				}
				else
				{
					currentline = "I'm leaving soon. Get as many people on board as possible. We need to hurry before the glare pierces through the windows!";
				}
				dialogueOptions.Add("Ok.");
				break;
			case SITUATION.BackedDownFromJailRequest:
				currentline = "Need anything else?";
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
				_numberOfWorkInterruptions++;
				if (optionID == 0)
				{
					nextContext = SITUATION.EscapeQuest;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.EscapeQuest:
				GameManager.Instance.TriggerEscapeQuest();
				amReadyToLeave = true;
				if (optionID != 4) GameManager.Instance.GoToGym();

				goto case SITUATION.PassiveChecks;
			case SITUATION.BackedDownFromJailRequest:
			case SITUATION.PassiveChecks:
			default:
				GameManager.Instance.playerLookingForBatteries = true;
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
		if (_gaveInfoAboutEscape) return SITUATION.EscapeQuest;
		return SITUATION.NormalGreating;
	}

	public int GetNPCID()
	{
		return 3;
	}
	public void InitNewDialogue()
	{
		currentContext = GetInitialContext();
		nextContext = currentContext;
		myData.Reset();
	}
}
