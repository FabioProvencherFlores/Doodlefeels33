using System.Collections.Generic;
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
	int _numberOfSmalltalk = 0;
	bool _alreadyaskedName = false;

	int _numberOfWarnings = 0;
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
				currentline = "You hungry?";
				dialogueOptions = new List<string> { "Who are you?", "What's cooking? It smells delicious." };
				break;
			case SITUATION.PlayerAskedName:
				if (_alreadyaskedName)
				{
					currentline = "I'm Harry fucking Potter. Now go!";
				}
				else
				{
					_alreadyaskedName = true;
					currentline = "Nowadays, I'm just the cook. That's fine by me.";
				}
				dialogueOptions = new List<string> { "Have you been here long?", "What are you doing?." };
				break;
			case SITUATION.BackedDownFromJailRequest:
				currentline = "I swear, you pull some stupid joke like that on me again...";
				dialogueOptions = new List<string> { "Have you been here long?", "What are you doing?." };
				break;
			case SITUATION.SmallTalk:
				if (_numberOfSmalltalk == 0)
				{
					currentline = "Listen, you seem fine and talky... but I got to feed y'all mouths.";
					dialogueOptions = new List<string> { "You're the chef, right?", "I bet it's gonna be real good!" };
				}
				else if (_numberOfSmalltalk == 1)
				{
					currentline = "Sure.";
					dialogueOptions = new List<string> { "What's your name, then?", "Glad we still got some food back there!", "Where are we?" };
				}
				else
				{
					currentline = "Hmph.";
					dialogueOptions = new List<string> { "I didn't get your name...", "", "Where are we?" };
				}
				_numberOfSmalltalk++;
				break;

			case SITUATION.PlayerAskedWhereAreWe:
				currentline = "You got sunblind, or what? Go bother that sleepy doctor over there instead.";
				break;
			case SITUATION.PlayerApologized:
				currentline = "Good.";
				break;
			case SITUATION.PlayerAskedToGoToJail:
				currentline = "What fuck, why?";
				removeGoodbye = true;
				dialogueOptions = new List<string> { "I Changed my mind...", "Get in the cell." };
				break;
			case SITUATION.AngryGreating:
				currentline = "You again. What do you want?";
				dialogueOptions = new List<string> { "Nevermind." };
				break;
			case SITUATION.PlayerAskedHowLong:
				currentline = "Been here a while. I think only that teacher was here before I was. Days get fuzzy, can't remember much.";
				dialogueOptions = new List<string> { "Good to know!" };
				break;
			case SITUATION.PlayerAskedWhatYouDoing:
				currentline = "I'm making diner. You blind?";
				dialogueOptions = new List<string> { "Ok, that's nice." };
				break;
			case SITUATION.NPCWarning:
				if (_numberOfWarnings == 0)
					currentline = "That lady is hysterical. You probably should do something about her, before I do.";
				else if (_numberOfWarnings == 1)
					currentline = "She's still screaming. Someone needs to shut her up.";
				else if (_numberOfWarnings == 2)
					currentline = "If you don't, I'll do it.";
				
				dialogueOptions.Add("I'm on it.");
				break;

		}

		return currentline;
	}

	bool _playerJustaskedName = false;
	bool _playerAskedWhatCooks = false;

	public void ProcessDialogueOption(int optionID)
	{
		_playerJustaskedName = false;
		_playerAskedWhatCooks = false;

		switch (currentContext)
		{
			case SITUATION.PlayerAskedName:
				myData.shouldGreatPlayer = false;
				if (optionID == 0)
				{
					nextContext = SITUATION.PlayerAskedHowLong;
				}
				else if (optionID == 1)
				{
					nextContext = SITUATION.PlayerAskedWhatYouDoing;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.SmallTalk:
				myData.shouldGreatPlayer = false;
				if (optionID == 0)
				{
					nextContext = SITUATION.PlayerAskedName;
				}
				else if (optionID == 1)
				{
					nextContext = SITUATION.SmallTalk;
				}
				else if (optionID == 2)
				{
					nextContext = SITUATION.PlayerAskedWhereAreWe;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedHowLong:
			case SITUATION.PlayerAskedWhatYouDoing:
				nextContext = SITUATION.SmallTalk;
				goto case SITUATION.PassiveChecks; ;
			case SITUATION.NormalGreating:
				myData.shouldGreatPlayer = false;
				if (optionID == 0)
				{
					nextContext = SITUATION.PlayerAskedName;
				}
				else if (optionID == 1)
				{
					nextContext = SITUATION.SmallTalk;
				}
				goto case SITUATION.PassiveChecks;

			case SITUATION.PlayerAskedToGoToJail:
				if (optionID == 0)
				{
					nextContext = SITUATION.BackedDownFromJailRequest;
					myData.playerWantsToJailMe = false;
				}
				else if (optionID == 1)
				{
					GameManager.Instance.PutCurrentNPCInJail();
					GameManager.Instance.GoToGym();
				}
				break;			
			case SITUATION.BackedDownFromJailRequest:
				if (optionID == 0)
				{
					nextContext = SITUATION.PlayerApologized;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.AngryGreating:
				_numberOfSmalltalk = 3;
				nextContext = SITUATION.SmallTalk;
				goto case SITUATION.PassiveChecks;
			case SITUATION.NPCWarning:
				_numberOfWarnings++;
				if (optionID == 0) nextContext = SITUATION.SmallTalk;
				goto case SITUATION.PassiveChecks;
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
		if (GameManager.Instance.isTeacherFreakingOut)
		{
			return SITUATION.NPCWarning;
		}
		if (myData.shouldGreatPlayer)
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

		return SITUATION.NormalGreating;
	}

	public void InitNewDialogue()
	{
		myData.shouldGreatPlayer = true;
		currentContext = GetInitialContext();
		nextContext	= currentContext;
		myData.Reset();
	}
}
