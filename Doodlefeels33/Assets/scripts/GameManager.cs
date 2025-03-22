using UnityEngine;


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


	[SerializeField]
    GameObject gymObjects;
    [SerializeField]
    GameObject dialogueObjects;

	public NPCController _currentNPC;

	private void Awake()
	{
		_instance = this;
	}

	private void Start()
	{
		GoToGym();
	}

	public void GoToGym()
    {
		gymObjects.SetActive(true);
		dialogueObjects.SetActive(false);
		_currentNPC = null;
	}

	public void SetNewNPC(NPCController aNPC)
	{
		_currentNPC = aNPC;
	}

    public void GoToDialogue()
    {
		gymObjects.SetActive(false);
		dialogueObjects.SetActive(true);

	}
}
