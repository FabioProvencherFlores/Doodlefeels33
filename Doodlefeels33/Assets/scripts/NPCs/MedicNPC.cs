using System;
using System.Collections.Generic;
using UnityEngine;

public class MedicNPC : BeigeNPC, IDialogue
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


	public bool _isTrustingTowardsPlayer = false;
	public bool _playerAskedToManyQuestions = false;
	public bool _willrefusetotalkuntiltomorrow = false;
	bool _hasExplainedQuestion = false;
	bool _consumedArtefact = false;
	public string GetNextDialogueString()
	{
		removeGoodbye = false;
		dialogueOptions.Clear();
		SITUATION previousSituation = currentContext;
		currentContext = nextContext;
		string currentline = "I SHOULD NOT SAY THIS, FAB MUST HAVE FORGOTTEN SOMETHING";

		if (_willrefusetotalkuntiltomorrow && currentContext != SITUATION.PlayerAskedToGoToJail)
		{
			currentline = "Go away...";
		}

		switch (currentContext)
		{
			case SITUATION.NormalGreating:
				if (amReadyToLeave)
				{
					currentline = "I'm packing my stuff. Samples are all packed. We will gatter more samples. Always more!";
				}
				else
				{
					if (!_isTrustingTowardsPlayer)
					{
						currentline = "What do you want?";
					}
					else
					{
						currentline = "Yes?	";
					}

				}
				dialogueOptions = new List<string> { "Who are you?", "Why are you standing up there?" };
				if (_isTrustingTowardsPlayer && !_hasExplainedQuestion)
					dialogueOptions.Add("How else can you help?");
				break;
			case SITUATION.ParanoiaNoneAnswer:
				removeGoodbye = true;
				currentline = "You sure ask a lot of questions for a new-comer. You won't get any anwers from me!";
				dialogueOptions = new List<string> { "Why? I'm just trying to help here.", "Why are you standing up there?", "I have this light headache..." };
				break;
			case SITUATION.PlayerAskedName:
				currentline = "Why do you want to know? Names don't mean nothing anymore. Forget them.";
				dialogueOptions = new List<string> { "I'm sorry if I'm intruding.", "Do you know more about sunblindness?" };
				break;
			case SITUATION.SmallTalk:
				currentline = "Yeah... I need to finish this. Let's talk later!";
				break;
			case SITUATION.PlayerASkedWhyYouHere:
				if (_isTrustingTowardsPlayer)
					currentline = "The air is pure, untouched. The Solar Eye floats on the ground. I am never ever touching the ground again, unless the night finally comes. Altough, I might just fall asleep if that happens.";
				else
					currentline = "The better question, why are you NOT!";
				dialogueOptions = new List<string> { "It must be windy up there!", "Do you know more about sunblindness?" };
				break;
			case SITUATION.BackedDownFromJailRequest:
				currentline = "Thank you. Thank you. Thank you. Thank you. Thank you. Thank you.";
				removeGoodbye = true;
				dialogueOptions = new List<string> { "You deserved it."
					, "Do you know more about sunblindness?"
					, "What was your name, before all this?"
					, "I'm sorry."};
				break;
			case SITUATION.PlayerInsultedMe:
				currentline = "Die of dysentry.";
					break;
			case SITUATION.PlayerApologized:
				currentline = "I know you don't mean harm, dear. It's just, with the disappearings and all... I think I need to rest.";
				break;
			case SITUATION.PlayerAskedToGoToJail:
				currentline = "What? No, please don't lock me in there!";
				removeGoodbye = true;
				dialogueOptions = new List<string> { "Okay, I won't.", "Please, don't make this difficult." };
				break;
			case SITUATION.PlayerAskedForInfo:
				if (GameManager.Instance.noOneDiedYet)
				{
					currentline = "I'm a medic, you know. I can't help healthy people. Come back when someone is injured... or worst.";
				}
				else if (_hasExplainedQuestion)
				{
					currentline = "The kind that will make the Solar Eye itself blink in terror. Now go!.";
				}
				else
				{
					removeGoodbye = true;
					currentline = "Dead people, really? You know... I might have an idea or two. It is never too late to cure one's body. Death is just another illness. If I am to help, I'll need an artefact... Ask the Hoarder over by the red doors.";
					dialogueOptions.Add("What kind of artefact?");
				}
				break;
			case SITUATION.ArtefactQuest:
				if (_consumedArtefact)
				{
					currentline = "It's done.";
				}
				else
				{
					removeGoodbye = true;
					currentline = "You have it, I see. The sun and the moon. Blinding light against deafening darkness. You may give it to me, and I shall cure the last trespassed, body and soul. Or, you keep it and its power may protect you, sometime. Think quick.";
					dialogueOptions.Add("Take it. Bring them back!");
					dialogueOptions.Add("I'll keep it... for now.");
				}
				break;
			case SITUATION.EscapeQuest:

					removeGoodbye = true;
					currentline = "Going outside? Out there?! No way";
					dialogueOptions.Add("You're right, we should stay.");
					dialogueOptions.Add("It's our only way, we are going to die here...");
				
				break;
			case SITUATION.PlayerAskedAboutSunblind:
				if (_isTrustingTowardsPlayer)
				{
					currentline = "The blindness! The sun, the moon. If you feel it, you should lock yourself in the dark. No light. Nothing. Two days!";
				}
				else
				{
					if (_playerAskedToManyQuestions)
					{
						currentline = "You ask an aweful lot of question...";
					}
					else
					{
						currentline = "I'm not telling you. Still got samples to taste, leave me be.";
					}
					dialogueOptions = new List<string> { "I am sorry for earlier. Please help me.", "I need to know." };
				}

				break;

		}


		return currentline;
	}
	bool _playerGaveArtefact = false;
	public void ProcessDialogueOption(int optionID)
	{

		switch (currentContext)
		{
			case SITUATION.NormalGreating:
				if (optionID == 0)
				{
					if (_isTrustingTowardsPlayer)
					{
						nextContext = SITUATION.PlayerAskedName;
					}
					else
					{
						nextContext = SITUATION.ParanoiaNoneAnswer;
					}
				}
				else if (optionID == 1)
				{
					nextContext = SITUATION.PlayerASkedWhyYouHere;
				}
				else if (optionID == 2)
				{
					nextContext = SITUATION.PlayerAskedForInfo;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.EscapeQuest:
				_isTrustingTowardsPlayer = true;
				_playerAskedToLeave = true;
				if (optionID == 1)
				{
					amReadyToLeave = true;
				}
				if (GameManager.Instance.playerhasArtefact && !_playerGaveArtefact) nextContext = SITUATION.ArtefactQuest;
				else nextContext = SITUATION.NormalGreating;
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedName:
				if (optionID == 0)
				{
					_isTrustingTowardsPlayer = true;
					nextContext = SITUATION.PlayerApologized;	
				}
				else if (optionID == 1)
				{
					nextContext = SITUATION.PlayerAskedAboutSunblind;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.ParanoiaNoneAnswer:
				if (optionID == 0)
				{
					_isTrustingTowardsPlayer = true;
					nextContext = SITUATION.NormalGreating;
				}
				if (optionID == 1)
				{
					nextContext = SITUATION.PlayerASkedWhyYouHere;
				}
				if (optionID == 2)
				{
					_isTrustingTowardsPlayer = true;
					nextContext = SITUATION.PlayerAskedAboutSunblind;
				}

				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerASkedWhyYouHere:
				if (optionID == 0)
				{
					nextContext = SITUATION.SmallTalk;
				}
				if (optionID == 1)
				{
					_playerAskedToManyQuestions = true;
					nextContext = SITUATION.PlayerAskedAboutSunblind;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedAboutSunblind:
				if (optionID == 0)
				{
					_isTrustingTowardsPlayer = true;
					_playerAskedToManyQuestions = false;
					nextContext = SITUATION.PlayerAskedAboutSunblind;
				}
				if (optionID == 1)
				{
					if (!_willrefusetotalkuntiltomorrow)
					{
						_willrefusetotalkuntiltomorrow = true;
						nextContext = SITUATION.ParanoiaNoneAnswer;
					}
					nextContext = SITUATION.PlayerInsultedMe;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedToGoToJail:
				_isTrustingTowardsPlayer = false;
				if (optionID == 0)
				{
					nextContext = SITUATION.BackedDownFromJailRequest;
				}
				if (optionID == 1)
				{
					GameManager.Instance.PutCurrentNPCInJail();
					GameManager.Instance.GoToGym();
					return;
				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.BackedDownFromJailRequest:
				if (optionID == 0)
				{
					_willrefusetotalkuntiltomorrow = true;
					nextContext = SITUATION.PlayerInsultedMe;
				}
				if (optionID == 1)
				{
					nextContext = SITUATION.PlayerAskedAboutSunblind;
				}
				if (optionID == 2)
				{
					nextContext = SITUATION.PlayerAskedName;
				}
				if (optionID == 3)
				{
					_isTrustingTowardsPlayer = true;
					nextContext = SITUATION.PlayerApologized;
				}
				break;
			case SITUATION.ArtefactQuest:
				if (_consumedArtefact)
				{
					GameManager.Instance.ResLastDead();
				}
				else
				{
					if (optionID == 0)
					{
						_consumedArtefact = true;
						GameManager.Instance.playerhasArtefact = false;
						nextContext = SITUATION.ArtefactQuest;
					}
					else if (optionID == 1)
					{
						nextContext = SITUATION.NormalGreating;
					}

				}
				goto case SITUATION.PassiveChecks;
			case SITUATION.PlayerAskedForInfo:
				if (optionID == 0)
				{
					GameManager.Instance.playerLookingForArtefact = true;
                    _hasExplainedQuestion = true;
                }
				goto case SITUATION.PassiveChecks;
			case SITUATION.SmallTalk:
			case SITUATION.PlayerApologized:
			case SITUATION.PlayerInsultedMe:
			case SITUATION.PassiveChecks:
				if (optionID == 3)
				{
					GameManager.Instance.GoToGym();
				}
				break;
			default:
				Debug.LogError("Dialogue state not supported: " + currentContext.ToString(), this);
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
	bool _playerAskedToLeave = false;
	public SITUATION GetInitialContext()
	{
		if (GameManager.Instance.npcsPrepareToLeave && !_playerAskedToLeave)
			return SITUATION.EscapeQuest;
		if (GameManager.Instance.playerhasArtefact && !_playerGaveArtefact)
			return SITUATION.ArtefactQuest;
		return SITUATION.NormalGreating;
	}

	public int GetNPCID()
	{
		return 1;
	}

	public void InitNewDialogue()
	{
		currentContext = GetInitialContext();
		nextContext = currentContext;
		myData.Reset();
	}
}
