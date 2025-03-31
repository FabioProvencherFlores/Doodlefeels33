using System;
using UnityEngine;

public class Kid1NPC : BeigeNPC, IDialogue
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
			case SITUATION.NPCAggroPlayer:
				removeGoodbye = true;
				currentline = "I told you I would kill you...";
				dialogueOptions.Add("Stop this nonesense!");
				dialogueOptions.Add("I won't hesitate to hurt a child.");
				dialogueOptions.Add("Calm down...");
				dialogueOptions.Add("You're crazy.");
				break;
			case SITUATION.NormalGreating:
				currentline = "I want to go outside!";
				break;
			case SITUATION.EscapeQuest:
				currentline = "Yeah, let's leave! Let's ALL go outside!!!";
				break;
			case SITUATION.PlayerAskedToGoToJail:
				removeGoodbye = true;
				currentline = "I'll kill you.";
				dialogueOptions.Add("Calm down. I was kidding!");
				dialogueOptions.Add("Get into the room. Now!");
				break;
			case SITUATION.AngryGreating:
			case SITUATION.BackedDownFromJailRequest:
				currentline = "...";
				break;
		}


		return currentline;
	}

	public bool kid1KilledPlayer = false;
	public void ProcessDialogueOption(int optionID)
	{
		switch (currentContext)
		{
			case SITUATION.NPCAggroPlayer:
				if (optionID == 1)
				{
					amDead = true;
					amMissing = true;
					GameManager.Instance.GoToGym();
				}
				else if (optionID != 4)
				{
					kid1KilledPlayer = true;
					GameManager.Instance.GoToGym();
					return;
				}
				else if (optionID == 4)
				{
					GameManager.Instance.PutCurrentNPCInJail();
					GameManager.Instance.GoToGym();
					return;
				}
				break;
			case SITUATION.EscapeQuest:
				amReadyToLeave = true;
				_playerAskedToLeave = true;
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedToGoToJail:
				myData.playerHasAskedForJail = true;
				GameManager.Instance.kid1WillKillMe = true;
				if (optionID == 0) nextContext = SITUATION.BackedDownFromJailRequest;
				if (optionID == 1)
				{
					GameManager.Instance.PutCurrentNPCInJail();
					GameManager.Instance.GoToGym();
					return;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.NormalGreating:
			case SITUATION.AngryGreating:
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
		if (GameManager.Instance.kid1WillKillMe && !myData.playerWantsToJailMe) return SITUATION.NPCAggroPlayer;
		if (myData.playerWantsToJailMe) return SITUATION.AngryGreating;
		return SITUATION.NormalGreating;
	}

	public int GetNPCID()
	{
		return 4;
	}

	public void InitNewDialogue()
	{
		currentContext = GetInitialContext();
		nextContext = currentContext;
		myData.Reset();
	}
}
