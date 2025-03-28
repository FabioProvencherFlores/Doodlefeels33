using UnityEngine;

public class AntiquarianNPC : BeigeNPC, IDialogue
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

	bool _askedAboutSymptoms = false;
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
				currentline = "We are all doomed and yet, here we stand in fear of the sun that made us surge with life.";
				if (GameManager.Instance.playerLookingForBatteries) dialogueOptions.Add("I'm looking for batteries. Got some?");
				if (GameManager.Instance.playerKnowsAboutKid2Fever && !_askedAboutSymptoms) dialogueOptions.Add("What would you do if you knew someone had fever symptoms?");
				break;
			case SITUATION.PlayerAskedToGoToJail:
				removeGoodbye= true;
				currentline = "You can't force me. I won't go in there. Better feel the caress of the sun one last time to live in enternal darkness.";
				dialogueOptions.Add("Don't go, stay with us.");
				dialogueOptions.Add("You are going in there.");
				break;
			case SITUATION.BackedDownFromJailRequest:
				currentline = "Ok.";
				if (GameManager.Instance.playerLookingForBatteries) dialogueOptions.Add("Nevermind that. Do you have batteries to spare?");
				break;
			case SITUATION.PlayerAskedForInfo:
				currentline = "What do you want me to tell you. I am no physician. You should ask the doctor over there. They are on that stupid bed that passes as a lab.";
				dialogueOptions.Add("What else can you tell me?.");
				dialogueOptions.Add("You seem scared.");
				break;
			case SITUATION.PlayerAskedAboutSunblind:
				if (GameManager.Instance.noOneDiedYet)
				{
					currentline = "You think we are safe here? The Eye slowly seeps in. No amount of wood and cardboard will ever stop it. Talk to the Seer. She knows what is to come. She sees it in the fire before we smell the smoke.";
				}
				else
				{
					currentline = "People already started to die, or disappear, right under your nose. We are all going to die, one at the time. Darkness is our solace, but we are not meant to live in shadows. What else is there?";
				}
				break;
			case SITUATION.NPCWarning:
				removeGoodbye = true;
				currentline = "And why wouldn't you be? The sun has decided to kill us. It drove us made. Do you seriously think we have a chance to escape the Solar Eye?";
				dialogueOptions.Add("Yes, we are safe here.");
				dialogueOptions.Add("We are gaining time, nothing more.");
				break;
			case SITUATION.PlayerASkedWhyYouHere:
				currentline = "We hid in here, and now we are stuck. Maybe the lucky ones are those that saw the Eye, like the Seer by the fire. If it wasn't for her, it'd be out there, roasted to death, or worse.";
				break;
			case SITUATION.PlayerAskedAboutBatteries:
				currentline = "What's in there for me? Ha, you know want... here they are. I'm tired of all this.";
				dialogueOptions.Add("Thank you...");
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
			case SITUATION.PlayerAskedAboutBatteries:
				GameManager.Instance.playerFoundBatteries = true;
				if (optionID != 4)
				{
					amMissing = true;
					amDead = true;
					GameManager.Instance.GoToGym();
					return;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.NormalGreating:
				if (optionID == 0)
				{
					if (GameManager.Instance.playerLookingForBatteries) nextContext = SITUATION.PlayerAskedAboutBatteries;
					else if (GameManager.Instance.playerKnowsAboutKid2Fever && !_askedAboutSymptoms) nextContext = SITUATION.PlayerAskedForInfo;
                }
				else if (optionID == 1)
				{
					if (GameManager.Instance.playerKnowsAboutKid2Fever && !_askedAboutSymptoms) nextContext = SITUATION.PlayerAskedForInfo;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.BackedDownFromJailRequest:
				if (GameManager.Instance.playerLookingForBatteries) nextContext = SITUATION.PlayerAskedAboutBatteries;
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedForInfo:
				_askedAboutSymptoms = true;
				if (optionID == 0) nextContext = SITUATION.PlayerAskedAboutSunblind;
				if (optionID == 1) nextContext = SITUATION.NPCWarning;
				goto case SITUATION.PassiveChecks;
			case SITUATION.NPCWarning:
				if (optionID == 0) nextContext = SITUATION.PlayerAskedAboutSunblind;
				if (optionID == 1) nextContext = SITUATION.PlayerASkedWhyYouHere;
				break;
			case SITUATION.PlayerASkedWhyYouHere:
			case SITUATION.PlayerAskedAboutSunblind:
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
		return 2;
	}
	public void InitNewDialogue()
	{
		currentContext = GetInitialContext();
		nextContext = currentContext;
		myData.Reset();
	}

}

