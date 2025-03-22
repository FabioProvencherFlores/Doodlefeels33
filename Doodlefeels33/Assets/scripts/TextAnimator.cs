using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAnimator : MonoBehaviour
{
	[TextArea(15, 20)]
	public string textToShow;
	public string currentTextShown;
	private string _oldTextShown;
	int _totalLength = -1;
	int _currentLength = -1;
	bool _isAnimating = false;
	bool _isMorph = false;

	[SerializeField]
	float timeBetweenTicks = 1f;
	float _timeSinceLastTick = 0f;

	[SerializeField]
	TMP_Text textUI;

	private void Awake()
	{
		_isAnimating = false;
		textUI = GetComponent<TMP_Text>();
	}
	private void Update()
	{
		if (_isAnimating)
		{
			if (_timeSinceLastTick > timeBetweenTicks)
			{
				_timeSinceLastTick = 0f;
				if (_currentLength <= _totalLength)
				{
					currentTextShown = textToShow.Substring(0, _currentLength);
					if (_isMorph)
					{
						if (_currentLength < _oldTextShown.Length)
						{
							currentTextShown += _oldTextShown.Substring(_currentLength);
						}
					}
					_currentLength++;
				}
				else
				{
					currentTextShown = textToShow;
					_isAnimating = false;
					_isMorph = false;
				}
			}
			else
			{
				_timeSinceLastTick += Time.deltaTime;
			}
		}

		textUI.text = currentTextShown;
	}

	public void SetAndStartAnimation(string aNewString)
	{
		textToShow = aNewString;
		StartAnimation();
	}

	public void StartAnimation()
	{
		_isAnimating = true;
		_currentLength = 0;
		if (textToShow.Length > 0) _currentLength = 1;
		_totalLength = textToShow.Length;
		_timeSinceLastTick = timeBetweenTicks + 1f;
		currentTextShown = "";
	}

	public void StartMorph()
	{
		_oldTextShown = currentTextShown;
		_isMorph = true;
		StartAnimation();
	}

	public void StopAnimation()
	{
		_isAnimating = false;
		textUI.text = "";
		currentTextShown = "";
	}
	public bool GetIsAnimating()
	{
		return _isAnimating;
	}

	public void ForceFullText()
	{
		_currentLength = _totalLength;
	}
}