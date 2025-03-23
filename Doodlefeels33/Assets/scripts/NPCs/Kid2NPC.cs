using System;
using System.Collections.Generic;
using UnityEngine;

public class Kid2NPC : BeigeNPC, IDialogue
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
	bool _playerKnowsName = false;
	bool _isFriendWithPlayer = false;
	public string GetNextDialogueString()
	{
		removeGoodbye = false;
		dialogueOptions.Clear();
		SITUATION previousSituation = currentContext;
		currentContext = nextContext;
		string currentline = "I SHOULD NOT SAY THIS, FAB MUST HAVE FORGOTTEN SOMETHING";

		currentContext = nextContext;
		switch (currentContext)
		{
			case SITUATION.NormalGreating:
				currentline = "Mommy don't want me to speak with strangers in her school.";
				dialogueOptions.Add("She won't know. I got some questions.");
				if (!_isFriendWithPlayer && GameManager.Instance.teacherKnowsPlayer)
				{
					string question = "Don't worry, she knows me. I'm a friend.";
					if (!_playerKnowsName)
						question += " What's your name?";
					dialogueOptions.Add(question);
				}
				if (_isFriendWithPlayer)
				{
					currentline = "Mommy says I need to sleep, but I'm not tired.";	
				}
				break;
			case SITUATION.AngryGreating:
				currentline = "No.";
				break;
			case SITUATION.PlayerAskedName:
				currentline = "Mommy calls me Timothy, but only when no one can hear.";
				_playerKnowsName = true;
				dialogueOptions.Add("Well Timothy, do you want to play?");
				dialogueOptions.Add("Careful no one else hears it. Do you know where we are?");
				break;
			case SITUATION.PlayerAskedToPlay:
				currentline = "Yes!!! ...but I need to rest or mommy is gonna be pissed off. Maybe another time?";
				break;
			case SITUATION.PlayerAskedWhereAreWe:
				currentline = "I used to come here. Mommy was my teacher. It was fun. Back from, you know, before the Solar Eye. Mommy doesn't like to talk about it...";
				break;
			case SITUATION.PlayerAskedToGoToJail:
				currentline = "I hate that room! Please, please, please, please. I can't breath in there! I'll be calm, I promise!";
				removeGoodbye = true;
				dialogueOptions = new List<string> { "Okay, I won't.", "Don't make me call your mother!" };
				break;

		}
		
		return currentline;
	}


	public void ProcessDialogueOption(int optionID)
	{
		switch (currentContext)
		{
			case SITUATION.NormalGreating:
				if (optionID == 0) nextContext = SITUATION.AngryGreating;
				if (optionID == 1) nextContext = SITUATION.PlayerAskedName;
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedName:
				if (optionID == 0)
				{
					_isFriendWithPlayer = true;
					nextContext = SITUATION.PlayerAskedToPlay;
				}
				if (optionID == 1) nextContext = SITUATION.PlayerAskedWhereAreWe;
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedToGoToJail:
				_isFriendWithPlayer = false;
				if (optionID == 0)
				{
					nextContext = SITUATION.AngryGreating;
				}
				if (optionID == 1)
				{
					GameManager.Instance.PutCurrentNPCInJail();
					GameManager.Instance.GoToGym();
					return;
				}
				break;
			case SITUATION.PassiveChecks:
			case SITUATION.PlayerAskedToPlay:
			case SITUATION.AngryGreating:
			case SITUATION.PlayerAskedWhereAreWe:
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
		return 5;
	}

	public void InitNewDialogue()
	{
		currentContext = GetInitialContext();
		nextContext = currentContext;
		myData.Reset();
	}
}
