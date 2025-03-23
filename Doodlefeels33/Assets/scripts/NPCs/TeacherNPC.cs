using System.Collections.Generic;
using UnityEngine;

public class TeacherNPC : BeigeNPC, IDialogue
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

	bool _isFriendlyTowardsPlayer = false;
	bool _isProtectiveToKid2 = true;
	bool _neverAskedJail = true;

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
			case SITUATION.AngryGreating:
				currentline = "Don't make me regret to letting you in.";
				dialogueOptions.Add("Don't worry, I mean no trouble.");
				dialogueOptions.Add("That's a nice kid you got there.");
				break;
			case SITUATION.PlayerAskedAboutKid2:
				if (_isFriendlyTowardsPlayer)
				{
					currentline = "Almost twelve, if you can believe it. I'm scared that for our safety. The Hunter protected us, now he's gone.";
				}
				else
				{
					currentline = "You don't need to get any closer. The Hunter may be gone, but I'm still here and will protect my child with everything I got.";
				}

				dialogueOptions.Add("It's gonna be alright. How are you feeling?");
				break;
			case SITUATION.PlayerAskedAboutSunblind:
				if (_isFriendlyTowardsPlayer)
				{
					currentline = "My baby... is having a fever, and mentionned a light headache. I'm too scared to ask for the doctor's help... It's rest. Some rest and it will all be okay.";
				}
				else
				{
					currentline = "It's... We are... Nevermind, we just need rest.";
					dialogueOptions.Add("Are you sure?");
				}
				break;
			case SITUATION.SmallTalk:
				currentline = "I'm sorry. Everyone seems a bit jumpy since the hunter disappeared. It's not your fault. I'm glad you are here.";
				dialogueOptions.Add("How did you get here?");
				dialogueOptions.Add("Is something wrong?");
				break;
			case SITUATION.PlayerASkedWhyYouHere:
				if (_isFriendlyTowardsPlayer)
				{
					currentline = "I used to teach here, before the... the Solar Eye. It's funny, we used to complain about the lack of sunlight in here...";
				}
				else
				{
					currentline = "Let's just say I'm from around here. That's all you need to know.";
				}
				dialogueOptions.Add("How old is your kid?");
				dialogueOptions.Add("Is something wrong?");
				break;
			case SITUATION.BackedDownFromJailRequest:
				currentline = "No matter what the others say, you are not welcomed here anymore.";
				break;
			case SITUATION.PlayerAskedToGoToJail:
				if (_neverAskedJail)
				{
					removeGoodbye = true;
					currentline = "Who do you think you are? You better leave.";
					dialogueOptions.Add("You're right.");
					dialogueOptions.Add("I'm in charge now, and I must insist.");
				}
				else
				{
					removeGoodbye = true;
					currentline = "You can't do that! Who's going to watch over my baby? You? Please, don't take me away from him. I beg you!";
					dialogueOptions.Add("Stay with your child.");
					dialogueOptions.Add("You need to follow me, for him.");

				}
				break;
		}


		return currentline;
	}

	public void ProcessDialogueOption(int optionID)
	{
		switch (currentContext)
		{
			case SITUATION.PlayerAskedToGoToJail:
				if (_neverAskedJail)
				{
					_isFriendlyTowardsPlayer = false;
					_neverAskedJail = false;
					if (optionID == 0)
					{
						nextContext = SITUATION.BackedDownFromJailRequest;
					}
					else if (optionID == 1)
					{
						nextContext = SITUATION.PlayerAskedToGoToJail;
					}
				}
				else
				{
					if (optionID == 0)
					{
						nextContext = SITUATION.BackedDownFromJailRequest;
					}
					else if (optionID == 1)
					{
						GameManager.Instance.PutCurrentNPCInJail();
						GameManager.Instance.GoToGym();
						return;
					}
				}
				break;
			case SITUATION.PlayerASkedWhyYouHere:
				GameManager.Instance.teacherKnowsPlayer = true;
				if (optionID == 0) nextContext = SITUATION.PlayerAskedAboutKid2;
				if (optionID == 1) nextContext = SITUATION.PlayerAskedAboutSunblind;
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedAboutKid2:
				GameManager.Instance.teacherKnowsPlayer = true;
				if (optionID == 0) nextContext = SITUATION.PlayerAskedAboutSunblind;
				goto case SITUATION.PassiveChecks;
			case SITUATION.SmallTalk:
				_isFriendlyTowardsPlayer = true;
				if (optionID == 0) nextContext = SITUATION.PlayerASkedWhyYouHere;
				if (optionID == 1) nextContext = SITUATION.PlayerAskedAboutSunblind;
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedAboutSunblind:
				_isFriendlyTowardsPlayer = true;
				if (optionID == 0) nextContext = SITUATION.SmallTalk;
				goto case SITUATION.PassiveChecks;
			case SITUATION.NormalGreating:
			case SITUATION.AngryGreating:
				if (optionID == 0) nextContext = SITUATION.SmallTalk;
				if (optionID == 1) nextContext = SITUATION.PlayerAskedAboutKid2;
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
		if (_isFriendlyTowardsPlayer)
			return SITUATION.NormalGreating;
		else
			return SITUATION.AngryGreating;
	}

	public int GetNPCID()
	{
		return 7;
	}

	public void InitNewDialogue()
	{
		currentContext = GetInitialContext();
		nextContext = currentContext;
		myData.Reset();
	}
}
