using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    TextAnimator textAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textAnimator.StartAnimation();
    }

}
