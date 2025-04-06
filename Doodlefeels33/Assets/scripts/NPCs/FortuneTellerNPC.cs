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

	bool _explainedPowers = false;
	public string GetNextDialogueString()
	{
		removeGoodbye = false;
		dialogueOptions.Clear();
		SITUATION previousSituation = currentContext;
		currentContext = nextContext;
		string currentline = "I SHOULD NOT SAY THIS, FAB MUST HAVE FORGOTTEN SOMETHING";


		switch (currentContext)
		{
			case SITUATION.PlayerAskedForInfo:
				if (_explainedPowers)
				{
					if (GameManager.Instance.isTeacherFreakingOut)
						currentline = "Watch out for the teacher. Her attitude is causing turmoil amongst our group. It may end in blood. The Solar Eye awaits blood. Make sure we don't give in.";
					else if (GameManager.Instance.isMonsterHungryTonight)
						currentline = "I see danger laying deep in the glare of the night. The Eye is staring at us. Someone will fall tonight if you don't take precautions.";
					else
						currentline = "The shadows cloak our eyes today. None will be harmed.";
				}
				else
				{
					currentline = "I can see the shadows move like the eyelashes of the blinking sun. They tell me what they forsee, what is to come, and what they see in our eyes. The Solar Eye sees all!";
				}
				break;
			case SITUATION.NormalGreating:
				currentline = "I miss the stars more than anything. What living is there for us if we can't look at the sky that saw us grow. When I look at the shadows, I see what the light used to tell us.";
				if (_explainedPowers) dialogueOptions.Add("What do you see?");
				else dialogueOptions.Add("What do you mean... The shadows talk to you?");
				break;
			case SITUATION.EscapeQuest:
				removeGoodbye = true;
				currentline = "I foresee a lot of Death outside. The Solar Eye is upon us, waiting. Are you sure you want us to leave?";
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
				if (optionID == 0) nextContext = SITUATION.PlayerAskedForInfo;
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedForInfo:
				_explainedPowers = true;
				goto case SITUATION.PassiveChecks;
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
