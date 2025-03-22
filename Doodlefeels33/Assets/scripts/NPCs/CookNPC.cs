using UnityEngine;

public class CookNPC : BeigeNPC, IDialogue
{
	[Header("Dialogue Data")]
	[SerializeField]
	public  Material customSprite;
	public Material spriteMaterial
	{
		get
		{
			return customSprite;
		}
	}

	bool removeGoodbye = false;
	public bool IsGoodbyeADefaultOption() { return !removeGoodbye; }

	public int GetNPCID()
	{
		return 0;
	}

	public override string GetJailLine()
	{
		if (myData.daysInPrison < 2)
		{
			return "Come on, won't you get hungry without me?";
		}

		else
		{
			return "Open the fucking door or I'll chop you into tiny darn pieces!";
		}
	}

	public string GetNextDialogueString()
	{
		removeGoodbye = false;
		SITUATION currentSituation = GetSituationID();
		string currentline = "I SHOULD NOT SAY THIS, FAB MUST HAVE FORGOTTEN SOMETHING";
		switch (currentSituation)
		{
			case SITUATION.NormalGreating:
				currentline = "You hungry?";
				dialogueOptions = new string[] { "Who are you?", "What's cooking? It smells delicious." };
				break;

			case SITUATION.AskedToGoToJail:
				currentline = "Are you sure?";
				removeGoodbye = true;
				dialogueOptions = new string[] { "I Changed my mind...", "Get in the cell." };
				break;

			case SITUATION.PassiveIdle:
				currentline = "What?";
				dialogueOptions = new string[] { "Let's start again"};
				break;
			case SITUATION.AngryGreating:
				currentline = "You again. What do you want?";
				dialogueOptions = new string[] { "Let's start again" };
				break;

		}

		return currentline;
	}

	public void ProcessDialogueOption(int optionID)
	{
		// 4 is jail
		if (optionID == 4)
		{
			myData.playerWantsToJailMe = true;
			myData.playerHasAskedForJail = true;
		}

		SITUATION currentSituation = GetSituationID();

		switch (currentSituation)
		{
			case SITUATION.NormalGreating:
				myData.shouldGreatPlayer = false;
				goto case SITUATION.PassiveIdle;

			case SITUATION.AskedToGoToJail:
				if (optionID == 0)
					myData.playerWantsToJailMe = false;
				else if (optionID == 1)
				{
					GameManager.Instance.PutCurrentNPCInJail();
					GameManager.Instance.GoToGym();
				}
				break;
			case SITUATION.AngryGreating:
			case SITUATION.PassiveIdle:
			default:
				if (optionID == 3)
				{
					GameManager.Instance.GoToGym();
				}
				break;
		}
	}

	public SITUATION GetSituationID()
	{
		if (myData.playerWantsToJailMe)
		{
			return SITUATION.AskedToGoToJail;
		}

		else if (myData.shouldGreatPlayer)
		{
			if (myData.playerHasAskedForJail)
			{
				return SITUATION.AngryGreating;
			}
			else
			{
				return SITUATION.NormalGreating;
			}
		}
		else
		{
			return SITUATION.PassiveIdle;
		}
	}

	public void InitNewDialogue()
	{
		myData.Reset();
	}
}
