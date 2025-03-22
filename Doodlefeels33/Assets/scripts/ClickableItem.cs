using UnityEngine;
using System.Collections;

public class ClickableItem : MonoBehaviour
{
	private bool _isHovered = false;
	private bool _isShowingHover = false;
	private float _lastHoverTime = 0f;
	[SerializeField]
	float hoverGracePeriod = 0.3f;
	[SerializeField]
	bool clickShouldGoToDialogue = true;


	[Header("Visual Feedback Stuff")]
	[SerializeField]
	Transform _spriteTransform;

	private IDialogue _npcController;

	private void Start()
	{
		_npcController = GetComponent<IDialogue>();
		_spriteTransform = gameObject.transform;
	}

	private void Update()
	{
		if (_isHovered)
		{
			if (_lastHoverTime + hoverGracePeriod < Time.time)
			{
				_isHovered = false;
			}
			else if (!_isShowingHover)
			{
				// switch to hover
				ChangeToHovered();
			}
		}
		else if (_isShowingHover)
		{
			// siwtch to idle
			ChangeToIdle();
		}


	}

	public void OnClick()
	{
		if (_npcController != null)
		{
			GameManager.Instance.SetNewNPC(_npcController);
		}
	}

	public bool ShouldGoToDialogue()
	{
		return clickShouldGoToDialogue;
	}

	private void ChangeToIdle()
	{
		print("change to idle");
		_spriteTransform.localScale = Vector3.one;
		_isShowingHover = false;
	}
	private void ChangeToHovered()
	{
		print("change to hover");
		_spriteTransform.localScale = Vector3.one * 1.1f;
		_isShowingHover = true;
	}

	public void HoverOverTimestamp()
	{
		_isHovered = true;
		_lastHoverTime = Time.time;
	}
}
