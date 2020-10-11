using UnityEngine;
using UnityEngine.UI;
using cakeslice;

public class LookAtClone : MonoBehaviour 
{
	public float distance = 200f;
	private Text hoverText;
	private bool lookingAtTarget = false;
	private bool actionToggle = true;
	private int lastLookNum;

	public GameObject hoverTextObject;
	public GameObject gameHUDControl;
	public GameObject soundsHUD;

	void Start () 
	{
		hoverText = hoverTextObject.GetComponent<Text>();
		hoverTextObject.gameObject.SetActive(false);
	}
	

	void Update () 
	{
		int layer_mask = LayerMask.GetMask("Ignore Raycast");

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;

		if (Physics.Raycast(ray, out hit, distance, layer_mask))
 		{
			if (hit.collider.tag == "RecordedPlayer")
			{
				lookingAtTarget = true;
				hoverText.text = ("GHOST - " + hit.transform.gameObject.GetComponent<InputVCR>().recordingNumber);
				//Debug.Log(hoverText.text);
				
				// if (hardInput.GetKeyDown("SwapPos"))
				// {
				// 	Vector3 myPos = gameObject.transform.position;
				// 	Vector3 cloneDistMoved = hit.transform.gameObject.transform.position - hit.transform.gameObject.GetComponent<InputVCR>().startPos;
				// 	gameObject.transform.position = hit.transform.gameObject.transform.position;
				// 	hit.transform.gameObject.transform.position = myPos;
				// 	hit.transform.gameObject.GetComponent<InputVCR>().startPos = hit.transform.gameObject.transform.position - cloneDistMoved;
				// }
			}
 		}
		else
		{
			lookingAtTarget = false;
		}

		if (lookingAtTarget)
		// && actionToggle)
		{
			hoverTextObject.SetActive(true);

			// for (int i = 0; i < gameHUDControl.GetComponent<TrackObjectOnUI>().WorldObjects.Count; i++)
			// {
			// 	if (gameHUDControl.GetComponent<TrackObjectOnUI>().WorldObjects[i] != null)
			// 	{
			// 		if (gameHUDControl.GetComponent<TrackObjectOnUI>().WorldObjects[i].name == hit.transform.name)
			// 		{
			// 			if (gameHUDControl.GetComponent<TrackObjectOnUI>().UI_Elements[i].gameObject.GetComponent<Animate>().anim != null)
			// 			{
			// 				soundsHUD.transform.Find("SelectionRingZoomIn").GetComponent<AudioSource>().Play();
			// 				gameHUDControl.GetComponent<TrackObjectOnUI>().UI_Elements[i].GetComponent<Animate>().DoAnimation();
			// 				actionToggle = false;
			// 			}
			// 			lastLookNum = i;
			// 		}
			// 	}
			// }		
		}
		else if (!lookingAtTarget)
		// && !actionToggle)
		{
			hoverTextObject.gameObject.SetActive(false);

			// soundsHUD.transform.Find("SelectionRingZoomOut").GetComponent<AudioSource>().Play();
			// try {gameHUDControl.GetComponent<TrackObjectOnUI>().UI_Elements[lastLookNum].GetComponent<Animate>().DoAnimation();}	
			// catch {Debug.Log("Selection Ring " + lastLookNum + " was not found, closing animation not played.");}
			// actionToggle = true;
		}
	}
}
