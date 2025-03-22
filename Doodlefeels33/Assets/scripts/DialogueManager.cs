using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    TextAnimator textAnimator;

    [SerializeField]
    MeshRenderer npcRenderer;

    private NPCController _npcController;

    void Start()
    {
		_npcController = GameManager.Instance._currentNPC;
		textAnimator.SetAndStartAnimation(_npcController.GetNextDialogueString());
		npcRenderer.material = _npcController.GetNPCMaterial();
    }

    public void InitDialogue(NPCController npcController)
    {
        _npcController = npcController;
        textAnimator.SetAndStartAnimation(npcController.GetNextDialogueString());
	}

}
