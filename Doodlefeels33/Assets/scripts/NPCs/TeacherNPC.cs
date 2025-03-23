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


	bool _isTrustingTowardsPlayer = false;
	bool _playerAskedToManyQuestions = false;

	public string GetNextDialogueString()
	{
		removeGoodbye = false;
		dialogueOptions.Clear();
		SITUATION previousSituation = currentContext;
		currentContext = nextContext;
		string currentline = "I SHOULD NOT SAY THIS, FAB MUST HAVE FORGOTTEN SOMETHING";




		return currentline;
	}

	public void ProcessDialogueOption(int optionID)
	{
		
	}

	public SITUATION GetInitialContext()
	{
		return SITUATION.NormalGreating;
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
