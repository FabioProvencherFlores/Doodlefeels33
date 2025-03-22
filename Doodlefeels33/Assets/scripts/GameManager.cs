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

	private void Awake()
	{
		_instance = this;
	}

	public void GoToGym()
    {
		gymObjects.SetActive(true);
		dialogueObjects.SetActive(false);

	}

    public void GoToDialogue()
    {
		gymObjects.SetActive(false);
		dialogueObjects.SetActive(true);
	}
}
