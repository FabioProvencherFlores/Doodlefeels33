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

}

public class BeigeNPC : MonoBehaviour, IDialogue
{
    [Header("Dialogue Data")]
    [SerializeField]
    Material customSprite;
	public Material spriteMaterial
	{
		get
		{
			return customSprite;
		}
	}

	private bool _hasSaidGreeting = false;
	string[] dialogueOptions;
	public string[] GetDialogueOptions() { return dialogueOptions; }
	string[] dialogueLines = new string[]
	{
		"Hello there!",
		"What?"
	};

    public string GetNextDialogueString()
    {
        if (!_hasSaidGreeting)
        {
			_hasSaidGreeting=true;
			dialogueOptions = new string[] { "Why is you?", "Goodbye" };
			return dialogueLines[0];

		}
		else
		{
			dialogueOptions = new string[] { "Let's start again", "Goodbye" };
			return dialogueLines[1];
		}

		return "I SHOULD NOT SAY THIS, I MUST HAVE FORGOTTEN SOMETHING";
    }

    public void InitNewDialogue()
    {
		_hasSaidGreeting = false;
	}
}

public class BlueNPC : MonoBehaviour, IDialogue
{
	[Header("Dialogue Data")]
	[SerializeField]
	Material customSprite;

	public Material spriteMaterial
	{
		get
		{
			return customSprite;
		}
	}

	private bool _hasSaidGreeting = false;

	public string GetNextDialogueString()
	{
		if (!_hasSaidGreeting)
		{

		}
		return "TEMP";
	}

	public void InitNewDialogue()
	{
		_hasSaidGreeting = false;
	}

	public string[] GetDialogueOptions()
	{
		throw new System.NotImplementedException();
	}
}

