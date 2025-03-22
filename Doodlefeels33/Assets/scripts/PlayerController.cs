using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private Ray ray;
	private RaycastHit hit;
	void Update()
	{
		bool clicked = false;
		if (Input.GetMouseButtonDown(0))
		{
			clicked = true;
		}

		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit))
		{
			ClickableItem click = hit.collider.gameObject.GetComponent<ClickableItem>();
			//Debug.Log("hit");
			if (click != null)
			{
				//print("clickable hit");
				click.HoverOverTimestamp();
				if (clicked && click.ShouldGoToDialogue())
				{
					click.OnClick();
					GameManager.Instance.GoToDialogue();
				}
			}
		}
	}
}
