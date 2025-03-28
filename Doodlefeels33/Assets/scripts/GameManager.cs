using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
	public bool isInDialogue = false;

	public IDialogue _currentNPC;

	[Header("All NPCs")]
	[SerializeField] public CookNPC _cookNPC;
	[SerializeField] public MedicNPC _medicNPC;
	[SerializeField] public AntiquarianNPC _antiquarianNPC;
	[SerializeField] public itNPC _itNPC;
	[SerializeField] public Kid1NPC _kid1NPC;
	[SerializeField] public Kid2NPC _kid2NPC;
	[SerializeField] public VeteranNPC _veteranNPC;
	[SerializeField] public TeacherNPC _teachNPC;
	[SerializeField] public FortuneTellerNPC _fortuneTellerNPC;
	[SerializeField] public ContructionNPC _contructionNPC;

	[Header("The killing")]
	[SerializeField]
	GameObject bloodSplatter;
	bool _didSomeoneDiedInJail = false;
	int daysSinceStart = 0;
	int daysWithoutInsident = 0;
	public bool playerLookingForBatteries = false;
	
	public bool playerFoundBatteries = false;
	public bool npcsPrepareToLeave = false;
	int escapeQuestStart = 0;

	public bool isMonsterHungryTonight = false;
	public bool playerKnowsAboutKid2Fever = false;

	public bool playerLookingForArtefact = false;
	public bool playerhasArtefact = false;

	BeigeNPC _lastDeath = null;
	public void TriggerEscapeQuest()
	{
		if (!npcsPrepareToLeave)
		{
			npcsPrepareToLeave = true;
			escapeQuestStart = daysSinceStart;
		}
	}

	public bool AreInteractionsRemaining()
	{
		return remainingInteractions > 0;
	}

	private void Awake()
	{
		playerFoundBatteries = false;
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

	public IEnumerator GotToFailureScreen()
	{
		yield return new WaitForSeconds(1);

		// change scene here
		Debug.Log(":(");
		SceneManager.LoadScene("GameoverScene");
	}

	public IEnumerator GoToWinScreen()
	{
		SceneManager.LoadScene("WinScene");
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

	public bool playerKnowsKid2Name = false;

	public void ResLastDead()
	{
		if (_lastDeath != null)
		{
			_lastDeath.amDead = false;
            _lastDeath.amMissing = false;
			_lastDeath.justGotRes = true;
			_lastDeath.amReadyToLeave = false ;
        }
	}

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

	public bool noOneDiedYet = true;
    void ProcessNight()
	{
		if (!_kid2NPC.amDead && _kid2NPC.amJailed)
		{
			if (_kid2NPC.nbNightsInPrison > 1)
			{
				Debug.Log("KID CURED");
				_kid2NPC.isCured = true;
			}
			_kid2NPC.nbNightsInPrison++;
		}
		else
		{
			_kid2NPC.nbNightsInPrison = 0;
		}


		bool noOneDied = true;
		if (daysSinceStart > 1 && isMonsterHungryTonight && !_kid2NPC.amDead && !_kid2NPC.isCured)
		{
			Debug.Log("killing starts!");


			BeigeNPC victim = ChooseNextDeath(_kid2NPC.amJailed);
			if (victim != null)
			{
				isMonsterHungryTonight = false;
                noOneDied = false;
				victim.amDead = true;
				victim.amMissing = true;
				if (victim.amJailed) _didSomeoneDiedInJail = true;
				_lastDeath = victim;
			}
		}
		else if (!isMonsterHungryTonight)
		{
			isMonsterHungryTonight = true;
        }

		if (isTeacherFreakingOut) nbDayHisteriaTeacher++;

		if (noOneDied)
		{
			if (nbDayHisteriaTeacher > 1)
			{
				if (!_cookNPC.amDead && !_teachNPC.amDead && _cookNPC.amJailed == _teachNPC.amJailed)
				{
					_teachNPC.amDead = true;
					_teachNPC.amMissing = true;
					if (_teachNPC.amJailed) _didSomeoneDiedInJail = true;

					noOneDied = false;
					_lastDeath = _teachNPC;
					_cookNPC.killedTeacher = true;
				}
			}
		}

		if (_veteranNPC.amJailed && !_veteranNPC.amDead && _veteranNPC.willKillNextJailNeibour)
		{
			foreach (BeigeNPC prisoner in jailedNPCs)
			{
				if (!prisoner.isVet)
				{
					prisoner.amDead = true;
					prisoner.amMissing= true;
					_didSomeoneDiedInJail = true;
					noOneDied = false;
					_veteranNPC.willKillNextJailNeibour = false;
                    _veteranNPC.killedOnCommand = true;


                }
			}
		}

		if (npcsPrepareToLeave)
		{
			if (escapeQuestStart + 2 < daysSinceStart)
			{
				if (_contructionNPC.amReadyToLeave && !_contructionNPC.amJailed)
				{
					_contructionNPC.amDead = true;
					_contructionNPC.amMissing = true;
				}
				if (_antiquarianNPC.amReadyToLeave && !_antiquarianNPC.amJailed)
				{
					_antiquarianNPC.amDead = true;
					_antiquarianNPC.amMissing = true;
				}
				if (_veteranNPC.amReadyToLeave && !_veteranNPC.amJailed)
				{
					_veteranNPC.amDead = true;
					_veteranNPC.amMissing = true;
				}
				if (_kid1NPC.amReadyToLeave && !_kid1NPC.amJailed)
				{
					_kid1NPC.amDead = true;
					_kid1NPC.amMissing = true;
				}
				if (_medicNPC.amReadyToLeave && !_medicNPC.amJailed)
				{
					_medicNPC.amDead = true;
					_medicNPC.amMissing = true;
				}
				if (_fortuneTellerNPC.amReadyToLeave && !_fortuneTellerNPC.amJailed)
				{
					_fortuneTellerNPC.amDead = true;
					_fortuneTellerNPC.amMissing = true;
				}
				if (_itNPC.amReadyToLeave && !_itNPC.amJailed)
				{
					_itNPC.amDead = true;
					_itNPC.amMissing = true;
				}
				if (_cookNPC.amReadyToLeave && !_cookNPC.amJailed)
				{
					_cookNPC.amDead = true;
					_cookNPC.amMissing = true;
				}
				if (_teachNPC.amReadyToLeave && !_teachNPC.amJailed)
				{
					if (!_teachNPC.amDead && !_kid2NPC.amJailed)
					{
						_kid2NPC.amDead = true;
						_kid2NPC.amMissing = true;
					}
					_teachNPC.amDead = true;
					_teachNPC.amMissing = true;
				}
			}
		}

		if (noOneDied)
		{
			daysWithoutInsident++;
		}
		else
		{
			noOneDiedYet = false;
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

		isInDialogue = true;
        remainingInteractions--;
		((IDialogue)_currentNPC).InitNewDialogue();
		gymObjects.SetActive(false);
		jailObjects.SetActive(false);
		dialogueObjects.SetActive(true);
		dialogueManager.InitDialogue((IDialogue)_currentNPC);
	}
	public void GoToGym()
	{
		isInDialogue = false;

        gymObjects.SetActive(true);
		jailObjects.SetActive(false);
		dialogueObjects.SetActive(false);
		_currentNPC = null;


		RefreshNPCPresence();
	}

	public void GoToJail()
	{
		isInDialogue = false;

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
}
