using System;
using UnityEngine;

public interface IDialogue
{
	public string GetNextDialogueString()
	{
		return "TEMP";
	}

    public void InitNewDialogue();
    public string[] GetDialogueOptions();
	public Material spriteMaterial { get; }

	public void ProcessDialogueOption(int optionID);
	public SITUATION GetSituationID();
	public int GetNPCID();
	public bool IsGoodbyeADefaultOption() {  return true; }
}

public class NPCData
{
	public bool shouldGreatPlayer = true;

	// jail data
	public bool playerWantsToJailMe = false;
	public bool playerHasAskedForJail = false;
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
	AskedToGoToJail,
	PassiveIdle,
}


public class BeigeNPC : MonoBehaviour
{

	NPCData myData;

	protected string[] dialogueOptions;
	public string[] GetDialogueOptions() { return dialogueOptions; }
	public bool amMissing = false;
	public bool amDead = false;

	public bool amJailed = false;

}
