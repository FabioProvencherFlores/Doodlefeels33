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


	Transform _spriteTransform;

	private IDialogue _npcController;
	private Vector3 startingScale;

	private AudioSource sfx_click;
	private AudioSource sfx_jail_click;
	private AudioSource sfx_hover;
	


	private void Awake()
	{
		startingScale = transform.localScale;
		
	}

	private void Start()
	{
		sfx_click = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
		sfx_jail_click = GameObject.FindGameObjectWithTag("chain_sfx").GetComponent<AudioSource>();
		sfx_hover = GameObject.FindGameObjectWithTag("hover_sfx").GetComponent<AudioSource>();
		_npcController = GetComponent<IDialogue>();
		_spriteTransform = gameObject.transform;
	}

	private void Update()
	{
		if (!GameManager.Instance.AreInteractionsRemaining())
		{
			return;
		}

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
			sfx_click.Play();
			GameManager.Instance.SetNewNPC(_npcController);
		}
		else
		{
			if(GameManager.Instance.jailedNPCs.Count > 0)
			{
				sfx_jail_click.Play();
			}
		}
	}

	public bool ShouldGoToDialogue()
	{
		return clickShouldGoToDialogue;
	}

	private void ChangeToIdle()
	{
		_spriteTransform.localScale = startingScale;
		_isShowingHover = false;
	}
	private void ChangeToHovered()
	{
		sfx_hover.Play();
		_spriteTransform.localScale = startingScale * 1.1f;
		_isShowingHover = true;
	}

	public void HoverOverTimestamp()
	{
		_isHovered = true;
		_lastHoverTime = Time.time;
	}
}
