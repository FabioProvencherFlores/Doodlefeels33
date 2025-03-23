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
		removeGoodbye = false;
		dialogueOptions.Clear();
		SITUATION previousSituation = currentContext;
		currentContext = nextContext;
		string currentline = "I SHOULD NOT SAY THIS, FAB MUST HAVE FORGOTTEN SOMETHING";


		switch (currentContext)
		{
			case SITUATION.NormalGreating:
				currentline = "We are all doomed and yet, here we stand in fear of the sun that made us surge with life.";
				break;
			case SITUATION.PlayerAskedToGoToJail:
				removeGoodbye= true;
				currentline = "You can't force me. I won't go in there. Better feel the caress of the sun one last time to live in enternal darkness.";
				dialogueOptions.Add("Don't go, stay with us.");
				dialogueOptions.Add("You are going in there.");
				break;
			case SITUATION.BackedDownFromJailRequest:
				currentline = "Ok.";
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
			case SITUATION.NormalGreating:
			case SITUATION.BackedDownFromJailRequest:
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

