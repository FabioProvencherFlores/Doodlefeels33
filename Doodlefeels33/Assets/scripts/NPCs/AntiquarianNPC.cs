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

	public string GetNextDialogueString()
	{
		return "";
	}

	public void InitNewDialogue()
	{
		
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

	public bool IsGoodbyeADefaultOption()
	{
		return true;
	}
}
