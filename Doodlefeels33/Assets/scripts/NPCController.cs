using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Dialogue Data")]
    [SerializeField]
    Material spriteMaterial;

    public string GetNextDialogueString()
    {
        return "TEMP";
    }

    public Material GetNPCMaterial()
    {
        return spriteMaterial;
    }
}
