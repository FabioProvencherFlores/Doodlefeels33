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
	[SerializeField]
	TextAnimator jailDialogue;
	BeigeNPC jailedNPC;

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
    [SerializeField]
    GameObject jailObjects;

	[Header("Dialogue Data")]
	[SerializeField]
	DialogueManager dialogueManager;

	public IDialogue _currentNPC;

	[Header("All NPCs")]
	[SerializeField] CookNPC _cookNPC;
	[SerializeField] MedicNPC _medicNPC;
	[SerializeField] TeacherNPC _teachNPC;

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
		if (_cookNPC == null)
		{
			Debug.LogError("Missing teacher", this);
		}
	}

	BeigeNPC GetNPCFromID(int id)
	{
		if (id == 0)
		{
			return _cookNPC;
		}
		if (id == 1)
		{
			return _medicNPC;
		}
		if (id == 7)
		{
			return _teachNPC;
		}

		Debug.LogError("You forgot the NPC ID", this);

		return null;
	}

	void StartNewDay()
	{
		// fade in
		blackSquareForNight.SetActive(false);
		remainingInteractions = nbOfInteractionsPerDay;
		_medicNPC._willrefusetotalkuntiltomorrow = false;
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
		jailedNPC = npc;
		npc.amMissing = true;
		npc.amJailed = true;
	}


	public void RefreshNPCPresence()
	{
		_hasJailedNPC = false;

		if (_cookNPC.amMissing)
		{
			_cookNPC.gameObject.SetActive(false);
			_hasJailedNPC |= _cookNPC.amJailed;
		}
		else
		{
			_cookNPC.gameObject.SetActive(true);
		}
		
		if (_teachNPC.amMissing)
		{
			_teachNPC.gameObject.SetActive(false);
			_hasJailedNPC |= _teachNPC.amJailed;
		}
		else
		{
			_teachNPC.gameObject.SetActive(true);
		}
		
		if (_medicNPC.amMissing)
		{
			_medicNPC.gameObject.SetActive(false);
			_hasJailedNPC |= _medicNPC.amJailed;
		}
		else
		{
			_medicNPC.gameObject.SetActive(true);
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
		jailObjects.SetActive(false);
		dialogueObjects.SetActive(true);
		dialogueManager.InitDialogue((IDialogue)_currentNPC);
	}
	public void GoToGym()
	{
		gymObjects.SetActive(true);
		jailObjects.SetActive(false);
		dialogueObjects.SetActive(false);
		_currentNPC = null;


		RefreshNPCPresence();
	}

	public void FreePrisonner()
	{
		jailedNPC.amMissing = false;
		jailedNPC.amJailed = false;
		jailedNPC = null;
		GoToGym();
	}

	public void GoToJail()
	{
		if (remainingInteractions < 1)
		{
			return;
		}

		remainingInteractions--;

		_currentNPC = null;
		gymObjects.SetActive(false);
		dialogueObjects.SetActive(false);
		jailObjects.SetActive(true);

		jailDialogue.SetAndStartAnimation("");
		jailDialogue.ForceFullText();

		if (jailedNPC != null)
		{
			jailDialogue.SetAndStartAnimation(jailedNPC.GetJailLine());
		}
		else
		{
			jailDialogue.SetAndStartAnimation("No one...");
		}

		(jailedNPC.myData.daysInPrison)++;
		print((jailedNPC.myData.daysInPrison));
	}
}
