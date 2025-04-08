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

	int _numberOfHellos = 0;
	public string GetNextDialogueString()
	{
		removeGoodbye = false;
		dialogueOptions.Clear();
		SITUATION previousSituation = currentContext;
		currentContext = nextContext;
		string currentline = "I SHOULD NOT SAY THIS, FAB MUST HAVE FORGOTTEN SOMETHING";

		if (GameManager.Instance.kid2WasJailedToday && !myData.playerWantsToJailMe)
		{
			_isFriendlyTowardsPlayer = false;
			GameManager.Instance.isTeacherFreakingOut = true;
			return "You emprisoned my baby you fucking psychopath! Get my child out of here right fucking now you fucking asshole! NOW!";
		}

		switch (currentContext)
		{
			case SITUATION.NormalGreating:
				if (_numberOfHellos == 0)
				{
					currentline = "That cook keeps looking at me funny...";
					_numberOfHellos++;
                }
				else if (_numberOfHellos == 1)
				{
					currentline = "Days are getting brighter...";
					_numberOfHellos++;

                }
				else if (_numberOfHellos == 2)
				{
					currentline = "The food situation is getting dire. We are running out of cans.";
					_numberOfHellos++;
				}
				else
				{
					currentline = "Yes?";
				}
				dialogueOptions.Add("You seem stressed...");
				dialogueOptions.Add("How did you find this place?");
                if (GameManager.Instance.playerLookingForBatteries && !GameManager.Instance.playerFoundBatteries) dialogueOptions.Add("I'm looking for batteries.");

                break;
			case SITUATION.PlayerAskedAboutBatteries:
				currentline = "Yes, got some in my handlight. I guess I could spare some of them if you really need them... Please bring them back to me right after. I need them for my baby to check... nevermind. Here, take them.";
                break;
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
                if (GameManager.Instance.playerLookingForBatteries && !GameManager.Instance.playerFoundBatteries) dialogueOptions.Add("I'm looking for batteries.");
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
			case SITUATION.EscapeQuest:
				if (amReadyToLeave)
				{
					currentline = "I'm packing. Can't wait to be out of this hell hole!";
					dialogueOptions.Add("You're doing great!");
				}
				else
				{
					removeGoodbye = true;
					currentline = "I heard about the escape plan. I'm not risking my baby out there for some idiot's dellusion";
					dialogueOptions.Add("You are right. We should stay here");
					if (GameManager.Instance.playerKnowsKid2Name) dialogueOptions.Add("You should go, both of you. Think of Thimoty...");
				}
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
	bool _playerAskedToLeave = false;
	public void ProcessDialogueOption(int optionID)
	{
		if (GameManager.Instance.kid2WasJailedToday && !myData.playerWantsToJailMe)
		{
			if (optionID != 4)
			{
				GameManager.Instance.GoToGym();
			}
		}
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
			case SITUATION.EscapeQuest:
				_playerAskedToLeave = true;
				if (optionID == 0) nextContext = SITUATION.SmallTalk;
				if (optionID == 1)
				{
					nextContext = SITUATION.EscapeQuest;
					amReadyToLeave = true;
				}
                GameManager.Instance.teacherKnowsPlayer = true;
                break;
			case SITUATION.PlayerAskedAboutKid2:
				GameManager.Instance.teacherKnowsPlayer = true;
				if (optionID == 0) nextContext = SITUATION.PlayerAskedAboutSunblind;
				goto case SITUATION.PassiveChecks;
			case SITUATION.SmallTalk:
				_isFriendlyTowardsPlayer = true;
				if (optionID == 0) nextContext = SITUATION.PlayerASkedWhyYouHere;
				if (optionID == 1) nextContext = SITUATION.PlayerAskedAboutSunblind;
				if (optionID == 2) nextContext = SITUATION.PlayerAskedAboutBatteries;
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedAboutSunblind:
				_isFriendlyTowardsPlayer = true;
				if (_isFriendlyTowardsPlayer) GameManager.Instance.playerKnowsAboutKid2Fever = true;
				if (optionID == 0) nextContext = SITUATION.SmallTalk;
				goto case SITUATION.PassiveChecks;
			case SITUATION.NormalGreating:
				if (optionID == 0) nextContext = SITUATION.SmallTalk;
				if (optionID == 1) nextContext = SITUATION.PlayerASkedWhyYouHere;
				if (optionID == 2) nextContext = SITUATION.PlayerAskedAboutBatteries;
				goto case SITUATION.PassiveChecks;
			case SITUATION.AngryGreating:
				if (optionID == 0) nextContext = SITUATION.SmallTalk;
				if (optionID == 1) nextContext = SITUATION.PlayerAskedAboutKid2;
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedAboutBatteries:
				GameManager.Instance.playerFoundBatteries = true;
				goto case SITUATION.PassiveChecks;
			case SITUATION.BackedDownFromJailRequest:
			case SITUATION.PassiveChecks:
				if (optionID == 3)
				{
					GameManager.Instance.GoToGym();
				}
				break;
			default:
				Debug.LogError("unsuported dialogue option: " + currentContext.ToString(), this);
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
		if (GameManager.Instance.npcsPrepareToLeave)
			return SITUATION.EscapeQuest;
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
