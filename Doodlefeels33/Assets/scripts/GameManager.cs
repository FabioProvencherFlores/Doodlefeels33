using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

	public static GameManager Instance
	{
		get
		{
			if (_instance is null)
				Debug.Log("Game Manager is broken woopsie");

			return _instance;
		}
	}

	[Header("Jail Objects")]
	[SerializeField]
	MeshRenderer jailVisual;
	bool _hasJailedNPC = false;
	[SerializeField]
	Material debugEmpty;
	[SerializeField]
	Material debugNotEmpty;

	[Header("Day night cycle")]
	[SerializeField]
	int nbOfInteractionsPerDay = 2;
	int remainingInteractions = 0;
	[SerializeField]
	GameObject goToBedButton;
	[SerializeField]
	GameObject blackSquareForNight;

	[Header("Scene management")]
	[SerializeField]
    GameObject gymObjects;
    [SerializeField]
    GameObject dialogueObjects;

	[Header("Dialogue Data")]
	[SerializeField]
	DialogueManager dialogueManager;

	public IDialogue _currentNPC;

	[Header("All NPCs")]
	[SerializeField] CookNPC _cookNPC;

	private void Awake()
	{
		_instance = this;
		if (jailVisual == null)
		{
			Debug.LogError("Missing jail visual", this);
		}
		if (_cookNPC == null)
		{
			Debug.LogError("Missing cook", this);
		}
	}

	BeigeNPC GetNPCFromID(int id)
	{
		if (id == 0)
		{
			return _cookNPC;
		}

		return null;
	}

	void StartNewDay()
	{
		// fade in
		blackSquareForNight.SetActive(false);
		remainingInteractions = nbOfInteractionsPerDay;
	}

	public void GoToSleep()
	{
		goToBedButton.SetActive(false);
		blackSquareForNight.SetActive(true);
		StartCoroutine(WaitNightThenStartNewDay());
	}

	void ProcessNight()
	{

	}

	IEnumerator WaitNightThenStartNewDay()
	{
		ProcessNight();
		yield return new WaitForSeconds(3);

		StartNewDay();
	}

	private void Start()
	{
		StartNewDay();
		GoToGym();
	}
	public void PutCurrentNPCInJail()
	{
		BeigeNPC npc = GetNPCFromID(_currentNPC.GetNPCID());
		npc.amMissing = true;
		npc.amJailed = true;
	}

	public void GoToGym()
    {
		gymObjects.SetActive(true);
		dialogueObjects.SetActive(false);
		_currentNPC = null;


		RefreshNPCPresence();
	}

	public void RefreshNPCPresence()
	{
		_hasJailedNPC = false;

		if (_cookNPC.amMissing)
		{
			_cookNPC.gameObject.SetActive(false);
			_hasJailedNPC |= _cookNPC.amJailed;
		}




		if (_hasJailedNPC)
		{
			jailVisual.material = debugNotEmpty;
		}
		else
		{
			jailVisual.material = debugEmpty;
		}


		if (remainingInteractions <= 0)
		{
			goToBedButton.SetActive(true);
		}

	}

	public void SetNewNPC(IDialogue aNPC)
	{
		_currentNPC = aNPC;
	}

    public void GoToDialogue()
    {
		if (remainingInteractions < 1)
		{
			return;
		}

		remainingInteractions--;
		((IDialogue)_currentNPC).InitNewDialogue();
		gymObjects.SetActive(false);
		dialogueObjects.SetActive(true);
		dialogueManager.InitDialogue((IDialogue)_currentNPC);
	}
}
