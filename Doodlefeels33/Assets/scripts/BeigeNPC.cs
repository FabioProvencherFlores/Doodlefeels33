using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogue
{
	public string GetNextDialogueString()
	{
		return "TEMP";
	}

    public void InitNewDialogue();
    public List<string> GetDialogueOptions();
	public Material spriteMaterial { get; }

	public void ProcessDialogueOption(int optionID);
	public SITUATION GetInitialContext();
	public int GetNPCID();
	public bool IsGoodbyeADefaultOption() {  return true; }
}

public class NPCData
{
	public bool shouldGreatPlayer = true;

	// jail data
	public bool playerWantsToJailMe = false;
	public bool playerHasAskedForJail = false;
	public int daysInPrison = 0;
	public void Reset()
	{
		playerWantsToJailMe = false;
		shouldGreatPlayer = true;
	}
}

public enum SITUATION
{
	INVALID,
	NormalGreating,
	AngryGreating,
	PassiveChecks,
	SmallTalk,
	BackedDownFromJailRequest,
	PlayerInsultedMe,
	ParanoiaNoneAnswer,
	PlayerApologized,
	PlayerAskedAboutSunblind,
	PlayerAskedName,
	PlayerAskedToGoToJail,
	PlayerAskedWhereAreWe,
	PlayerAskedHowLong,
	PlayerAskedWhatYouDoing,
	PlayerASkedWhyYouHere,
	PlayerAskedToPlay,
	PlayerAskedForInfo,
	PlayerAskedAboutKid2,
	PlayerAskedAboutDisappearance,
	NPCWarning,
	NPCAggroPlayer,
	PlayerAskedAboutBatteries,
	EscapeQuest,
	ArtefactQuest,
	PostResDialogue
}


public class BeigeNPC : MonoBehaviour
{

	public NPCData myData = new NPCData();

	protected List<string> dialogueOptions = new List<string>{ "" };
	public List<string> GetDialogueOptions() { return dialogueOptions; }
	public bool amMissing = false;
	public bool amDead = false;

	public bool amJailed = false;

	public bool amReadyToLeave = false;

	public bool justGotRes = false;
	public bool amMurderedInGym = false;

	protected SITUATION currentContext;
	protected SITUATION nextContext;

	protected bool removeGoodbye = false;
	public bool isVet = false;
	public bool IsGoodbyeADefaultOption()
	{
		return !removeGoodbye;
	}

	public bool isLeavingToGym = false;
	public void LeaveToGym()
	{
		isLeavingToGym = true;
        GameManager.Instance.GoToGym();
    }

	public virtual string GetJailLine() { return "I got nothing to say to you..."; }

}
