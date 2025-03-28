using System;
using UnityEngine;

public class VeteranNPC : BeigeNPC, IDialogue
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

	bool _suspectedGroupedPeople = false;
	bool _suspectedCornerPeople = false;

	bool _playerAskedToLeave = false;
	int saidBackstory = 0;

	public bool willKillNextJailNeibour = false;
	public bool killedOnCommand = false;

    private void Awake()
    {
		isVet = true;
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
			case SITUATION.NormalGreating:
				if (killedOnCommand)
				{
					killedOnCommand = false;
					currentline = "I did as you asked, captain. I would do it again.";
				}
				else if (amReadyToLeave)
				{
					currentline = "Anything else before we leave?";
				}
				else
				{
					currentline = "Yes captain?";
					dialogueOptions.Add("Noticed anything strange to report?");
					dialogueOptions.Add("I have a strange feeling. What type of signs should I be looking for?");
					if (!GameManager.Instance.noOneDiedYet) dialogueOptions.Add("I noticed some people are missing all of the sudden.");
				}
				break;
			case SITUATION.PlayerAskedForInfo:
				if (GameManager.Instance.noOneDiedYet)
				{
					currentline = "Not much, since the Hunter disappeared. I'll keep you posted if I hear anything out of the ordinary.";
				}
				else
				{
					if (_suspectedGroupedPeople)
					{
                        currentline = "Precisely. Be wary of those folks.";
					}	
					else if (_suspectedCornerPeople)
					{
						currentline = "Not sure. They may certainly have more information to provide.";
					}
					else
					{
						currentline = "I heard some strange noises at night coming from the folks over there. You should check it surely out.";
						dialogueOptions.Add("You mean that solitary lad over there?");
						dialogueOptions.Add("You're refering to the group in the corner?");
					}
				}
				break;
			case SITUATION.PlayerASkedWhyYouHere:
				print(saidBackstory);
				if (saidBackstory == 0)
				{
					currentline = "I haven't always been old, y'know. Let's just say I've seen my fair share of death in my carrier.";
					saidBackstory++;
                }
				else if (saidBackstory == 1)
				{
					currentline = "I was in the trained forced. I've done my duty. I have killed before. Don't worry, I'm not planning in steping out of retirement.";
					saidBackstory++;
					dialogueOptions.Add("Do you think you could do an exception, just this one time?");
                }
				else if (saidBackstory == 2)
				{
					currentline = "When the Solar Eye awoke, I was at the park. Luckily I suffered a heat strike and passed out... I've seen what the Eye can do, how it changes people... They aren't people anymore."; 
					dialogueOptions.Add("Then, you know how dangerous it is to be here with them. I need some help with the sunblind people.");
					saidBackstory++;
                }
				else
				{
					currentline = "Believe me, I wish I didn't. Anything else?";
				}

				break;
			case SITUATION.PlayerAskedAboutSunblind:
				currentline = "I've seen strange behaviours in the Outside times. Scary ones, at that. It usually manifests with light fever and headaches. Maybe the doctor up there can give you more information?";
				if (saidBackstory == 0) dialogueOptions.Add("You seem pretty knowledgeable...");
				else dialogueOptions.Add("Tell me more of yourself. What else have you seen?");
				break;
			case SITUATION.PlayerAskedAboutDisappearance:
				currentline = "If there's blood, there's bad omen. Someone among us is doing this. We need to find them... quick.";
				dialogueOptions.Add("How do you know all this?");
				dialogueOptions.Add("I'm concerned about it too. Anyone I should check?");
				dialogueOptions.Add("We need to put an end to this. You, and I. Will you help me?");
				break;
			case SITUATION.EscapeQuest:
				removeGoodbye= true;
				currentline = "I'll follow you, captain. Just tell me what is best!";
				dialogueOptions.Add("It's best if we stay here.");
				dialogueOptions.Add("You should pack light, there is a long road ahead.");
				break;
			case SITUATION.NPCWarning:
				currentline = "I hear you crystal clear. Don't ask for details. Just put me in that room with whoever you need me to fix, and I'll fix them. They'll be out of your hands. Just this one time... Go ahead captain, chain me up.";
				break;
			case SITUATION.PlayerAskedToGoToJail:
				removeGoodbye = true;
				currentline = "If you say so. I will await for further instructions.";
				dialogueOptions.Add("On second thought, stand guard with us.");
				dialogueOptions.Add("Thanks for your cooperation.");
				break;
			case SITUATION.BackedDownFromJailRequest:
				currentline = "As you say.";
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
					LeaveToGym();
					return;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.EscapeQuest:
				if (optionID == 1)
				{
					amReadyToLeave = true;
				}
				nextContext = SITUATION.NormalGreating;
				break;
			case SITUATION.NormalGreating:
				if (optionID == 0) nextContext = SITUATION.PlayerAskedForInfo;
				else if (optionID == 1) nextContext = SITUATION.PlayerAskedAboutSunblind;
				else if (optionID == 2) nextContext = SITUATION.PlayerAskedAboutDisappearance;
                goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedForInfo:
				if (optionID == 0)
				{
					nextContext = SITUATION.PlayerAskedForInfo;
                    _suspectedCornerPeople = true;
				}
				else if (optionID == 1)
				{
					nextContext = SITUATION.PlayerAskedForInfo;
					_suspectedGroupedPeople = true;
				}
				else
				{
					_suspectedGroupedPeople = false;
					_suspectedCornerPeople = false;
				}
                goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedAboutDisappearance:
				if (optionID == 0) nextContext = SITUATION.PlayerASkedWhyYouHere;
				if (optionID == 1) nextContext = SITUATION.PlayerAskedForInfo;
				if (optionID == 2) nextContext = SITUATION.NPCWarning;
                goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedAboutSunblind:
				if (optionID == 0) nextContext = SITUATION.PlayerASkedWhyYouHere;
                goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerASkedWhyYouHere:
				if (optionID == 0) nextContext = SITUATION.NPCWarning;
                goto case SITUATION.PassiveChecks;
			case SITUATION.NPCWarning:
				willKillNextJailNeibour = true;
                goto case SITUATION.PassiveChecks;
			case SITUATION.BackedDownFromJailRequest:
			case SITUATION.PassiveChecks:
				if (optionID == 3)
				{
                    LeaveToGym();
                }
				break;
			default:
				Debug.LogError("Unsuported state in dialogue.", this);
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
		if (GameManager.Instance.npcsPrepareToLeave && !_playerAskedToLeave)
			return SITUATION.EscapeQuest;
		return SITUATION.NormalGreating;
	}


	public int GetNPCID()
	{
		return 6;
	}
	public void InitNewDialogue()
	{
		currentContext = GetInitialContext();
		nextContext = currentContext;
		myData.Reset();
	}
}
