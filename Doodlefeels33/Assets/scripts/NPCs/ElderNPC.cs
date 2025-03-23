using System;
using UnityEngine;

public class ElderNPC : BeigeNPC, IDialogue
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

	public string GetNextDialogueString()
	{
		return "I shouldn't exist, anymore";
	}


	public void ProcessDialogueOption(int optionID)
	{

	}

	public SITUATION GetInitialContext()
	{
		return SITUATION.INVALID;
	}

	public int GetNPCID()
	{
		return -1;
	}

	public void InitNewDialogue()
	{
		currentContext = GetInitialContext();
		nextContext = currentContext;
		myData.Reset();
	}

}
