using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public List<BeigeNPC> jailedNPCs = new List<BeigeNPC>();

	[Header("Day night cycle")]
	[SerializeField]
	int nbOfInteractionsPerDay = 2;
	int remainingInteractions = 0;
	[SerializeField]
	GameObject goToBedButton;
	[SerializeField]
	GameObject blackSquareForNight;
	[SerializeField]
	GameObject redSquareForDeath;

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
	[SerializeField] AntiquarianNPC _antiquarianNPC;
	[SerializeField] itNPC _itNPC;
	[SerializeField] Kid1NPC _kid1NPC;
	[SerializeField] Kid2NPC _kid2NPC;
	[SerializeField] VeteranNPC _veteranNPC;
	[SerializeField] TeacherNPC _teachNPC;
	[SerializeField] FortuneTellerNPC _fortuneTellerNPC;
	[SerializeField] ContructionNPC _contructionNPC;

	[Header("The killing")]
	[SerializeField]
	GameObject bloodSplatter;
	bool _didSomeoneDiedInJail = false;
	int daysSinceStart = 0;
	int daysWithoutInsident = 0;

	public bool AreInteractionsRemaining()
	{
		return remainingInteractions > 0;
	}

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
		if (id == 2) return _antiquarianNPC;
		if (id == 3) return _itNPC;
		if (id == 4) return _kid1NPC;
		if (id == 5)
		{
			return _kid2NPC;
		}
		if (id == 6) return _veteranNPC;
		if (id == 7)
		{
			return _teachNPC;
		}
		if (id == 8) return _fortuneTellerNPC;
		if (id == 9) return _contructionNPC;

		Debug.LogError("You forgot the NPC ID", this);

		return null;
	}

	public bool IsMorning() { return remainingInteractions > 0; }
	public bool IsEvening() { return remainingInteractions == 0; }

	public bool kid1WillKillMe = false;

	void StartNewDay()
	{
		// fade in
		blackSquareForNight.SetActive(false);
		remainingInteractions = nbOfInteractionsPerDay;
		_medicNPC._willrefusetotalkuntiltomorrow = false;
		kid2WasJailedToday = _kid2NPC.amJailed;
		daysSinceStart++;

		bool everyoneDied = true;
		if (!_contructionNPC.amDead) everyoneDied = false;
		if (!_antiquarianNPC.amDead) everyoneDied = false;
		if (!_veteranNPC.amDead) everyoneDied = false;
		if (!_kid1NPC.amDead) everyoneDied = false;
		if (!_medicNPC.amDead) everyoneDied = false;
		if (!_fortuneTellerNPC.amDead) everyoneDied = false;
		if (!_itNPC.amDead) everyoneDied = false;
		if (!_cookNPC.amDead) everyoneDied = false;
		if (!_teachNPC.amDead) everyoneDied = false;

		if (everyoneDied)
		{
			redSquareForDeath.SetActive(true);
			StartCoroutine(GotToFailureScreen());
			return;
		}

		if (daysWithoutInsident >= 3)
		{
			StartCoroutine(GoToWinScreen());
			return;
		}

		if (kid1WillKillMe && !_kid1NPC.amDead && !_kid1NPC.amJailed)
		{
			_currentNPC = _kid1NPC;
			_kid1NPC.InitNewDialogue();
			GoToDialogue();

			return;
		}

		GoToGym();
	}

	public IEnumerator GotToFailureScreen()
	{
		yield return new WaitForSeconds(1);

		// change scene here
		Debug.Log(":(");
	}

	public IEnumerator GoToWinScreen()
	{
		yield return new WaitForSeconds(1);
		
		// change scene here
		Debug.Log(":)");
	}

	public void GoToSleep()
	{
		goToBedButton.SetActive(false);
		blackSquareForNight.SetActive(true);
		StartCoroutine(WaitNightThenStartNewDay());
	}

	int nbDayHisteriaTeacher = 0;

	void ProcessNight()
	{
		bool noOneDied = true;
		if (daysSinceStart > 1 && daysSinceStart %2 == 0 && !_kid2NPC.amDead)
		{
			Debug.Log("killing starts!");
			daysWithoutInsident = 0;

			BeigeNPC victim = ChooseNextDeath(_kid2NPC.amJailed);
			if (victim != null)
			{
				noOneDied = false;
				victim.amDead = true;
				victim.amMissing = true;
				if (victim.amJailed) _didSomeoneDiedInJail = true;
			}
		}

		if (isTeacherFreakingOut) nbDayHisteriaTeacher++;

		if (noOneDied)
		{
			if (nbDayHisteriaTeacher > 2)
			{
				if (!_cookNPC.amDead && !_teachNPC.amDead && _cookNPC.amJailed == _teachNPC.amJailed)
				{
					_teachNPC.amDead = true;
					_teachNPC.amMissing = true;
					if (_teachNPC.amJailed) _didSomeoneDiedInJail = true;

					noOneDied = false;
				}
			}
		}

		if (noOneDied)
		{
			daysWithoutInsident++;
		}
		else
		{
			daysWithoutInsident = 0;
		}
	}

	BeigeNPC ChooseNextDeath(bool inJail)
	{
		if (!_contructionNPC.amDead && _contructionNPC.amJailed == inJail) return _contructionNPC;
		if (!_antiquarianNPC.amDead && _antiquarianNPC.amJailed == inJail) return _antiquarianNPC;
		if (!_veteranNPC.amDead && _veteranNPC.amJailed == inJail) return _veteranNPC;
		if (!_kid1NPC.amDead && _kid1NPC.amJailed == inJail) return _kid1NPC;
		if (!_medicNPC.amDead && _medicNPC.amJailed == inJail) return _medicNPC;
		if (!_fortuneTellerNPC.amDead && _fortuneTellerNPC.amJailed == inJail) return _fortuneTellerNPC;
		if (!_itNPC.amDead && _itNPC.amJailed == inJail) return _itNPC;
		if (!_cookNPC.amDead && _cookNPC.amJailed == inJail) return _cookNPC;
		if (!_teachNPC.amDead && _teachNPC.amJailed == inJail) return _teachNPC;

		return null;
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
	}
	public void PutCurrentNPCInJail()
	{
		BeigeNPC npc = GetNPCFromID(_currentNPC.GetNPCID());
		jailedNPCs.Add(npc);
		npc.amMissing = true;
		npc.amJailed = true;
	}

	public bool kid2WasJailedToday = false;
	public bool isTeacherFreakingOut = false;

	public void RefreshNPCPresence()
	{
		if (_kid1NPC.kid1KilledPlayer)
		{
			redSquareForDeath.SetActive(true);
			StartCoroutine(GotToFailureScreen());
		}

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
		
		if (_medicNPC.amMissing)
		{
			_medicNPC.gameObject.SetActive(false);
			_hasJailedNPC |= _medicNPC.amJailed;
		}
		else
		{
			_medicNPC.gameObject.SetActive(true);
		}

		if (_antiquarianNPC.amMissing)
		{
			_antiquarianNPC.gameObject.SetActive(false);
			_hasJailedNPC |= _antiquarianNPC.amJailed;
		}
		else
		{
			_antiquarianNPC.gameObject.SetActive(true);
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

		if (_itNPC.amMissing)
		{
			_itNPC.gameObject.SetActive(false);
			_hasJailedNPC |= _itNPC.amJailed;
		}
		else
		{
			_itNPC.gameObject.SetActive(true);
		}

		if (_kid1NPC.amMissing)
		{
			_kid1NPC.gameObject.SetActive(false);
			_hasJailedNPC |= _kid1NPC.amJailed;
		}
		else
		{
			_kid1NPC.gameObject.SetActive(true);
		}

		if (_kid2NPC.amMissing)
		{
			_kid2NPC.gameObject.SetActive(false);
			_hasJailedNPC |= _kid2NPC.amJailed;
		}
		else
		{
			_kid2NPC.gameObject.SetActive(true);
		}

		if (_veteranNPC.amMissing)
		{
			_veteranNPC.gameObject.SetActive(false);
			_hasJailedNPC |= _veteranNPC.amJailed;
		}
		else
		{
			_veteranNPC.gameObject.SetActive(true);
		}

		if (_fortuneTellerNPC.amMissing)
		{
			_fortuneTellerNPC.gameObject.SetActive(false);
			_hasJailedNPC |= _fortuneTellerNPC.amJailed;
		}
		else
		{
			_fortuneTellerNPC.gameObject.SetActive(true);
		}
		if (_contructionNPC.amMissing)
		{
			_contructionNPC.gameObject.SetActive(false);
			_hasJailedNPC |= _contructionNPC.amJailed;
		}
		else
		{
			_contructionNPC.gameObject.SetActive(true);
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

		if (_didSomeoneDiedInJail)
		{
			bloodSplatter.SetActive(true);
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
		bool kid2WasJailed = _kid2NPC.amJailed;
		foreach (BeigeNPC prisoner in jailedNPCs)
		{
			if (!prisoner.amDead) prisoner.amMissing = false;
			prisoner.amJailed = false;
		}
		jailedNPCs.Clear();

		if (kid2WasJailed && !_kid2NPC.amJailed)
		{
			isTeacherFreakingOut = false;
			nbDayHisteriaTeacher = 0;
		}
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

		if (jailedNPCs.Count > 0)
		{
			jailDialogue.SetAndStartAnimation(jailedNPCs[0].GetJailLine());
		}
		else
		{
			jailDialogue.SetAndStartAnimation("No one...");
		}
		foreach (BeigeNPC prisoner in jailedNPCs)
		{
			(prisoner.myData.daysInPrison)++;
		}
	}

	public bool teacherKnowsPlayer = false;
}
