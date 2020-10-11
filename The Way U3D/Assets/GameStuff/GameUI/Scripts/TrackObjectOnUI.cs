using System.Collections.Generic;
using UnityEngine;

public class TrackObjectOnUI : MonoBehaviour 
{

	//This is your object that you want to have the UI element hovering over
	[HideInInspector]
	public List <GameObject> WorldObjects = new List<GameObject>();
	private GameObject WorldObject;

	//This is the UI element
	[HideInInspector]
	public List <GameObject> UI_Elements = new List<GameObject>();
	public GameObject UI_Element;

	//Other
	private Canvas targetCanvas;

	private Vector3 trackHightOffset = new Vector3(0.0f, 1.0f, 0.0f);

	void Awake () 
	{
		WorldObjects.Clear();
		UI_Elements.Clear();
		targetCanvas = gameObject.GetComponent<Canvas>();
	}

	void Update () 
	{
		if (WorldObjects.Count > 0)
		{
			for (int i = 0; i < WorldObjects.Count; i++)
			{
				bool isFoundTemp = false;
				for (int j = 0; j < GameObject.FindGameObjectsWithTag("RecordedPlayer").Length; j++)
				{
					if (WorldObjects[i] != null)
					{
						isFoundTemp = true;
					}
					break;
				}
				if (!isFoundTemp)
				{
					WorldObjects.Remove(WorldObjects[i].gameObject);
					UI_Elements[i].GetComponent<EMSimpleMotion>().RewindMotion();
					Destroy(UI_Elements[i].gameObject, 0.4f);
					UI_Elements.Remove(UI_Elements[i]);
				}
			}
		}

		if (GameObject.FindGameObjectWithTag("RecordedPlayer"))
		{
			for (int i = 0; i < GameObject.FindGameObjectsWithTag("RecordedPlayer").Length; i++)
			{
				if (!WorldObjects.Contains(GameObject.FindGameObjectsWithTag("RecordedPlayer")[i]))
				{
					WorldObjects.Add(GameObject.FindGameObjectsWithTag("RecordedPlayer")[i]);
					UI_Elements.Add(Instantiate(UI_Element));
					UI_Elements[i].transform.SetParent(targetCanvas.transform, false);
				}
			}
			
			for (int i = 0; i < WorldObjects.Count; i++)
			{
				//Debug.Log(i);
				//Debug.Log(WorldObjects.Count);

				// First you need the RectTransform component of your canvas
				RectTransform CanvasRect = targetCanvas.GetComponent<RectTransform>();

                // Then you calculate the position of the UI element
                // 0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
                try
                {
                    Vector3 ViewportPosition = Camera.main.WorldToViewportPoint(WorldObjects[i].transform.position + trackHightOffset);

                    if (ViewportPosition.x <= 1 && ViewportPosition.x >= 0 && ViewportPosition.y <= 1 && ViewportPosition.y >= 0 && ViewportPosition.z >= 0)
                    {
                        Vector2 WorldObject_ScreenPosition = new Vector2(
                        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
                        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

                        // Now you can set the position of the ui element
                        UI_Elements[i].GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
                        if (UI_Elements[i].GetComponent<Animate>().anim != null)
                        {
                            UI_Elements[i].GetComponent<EMSimpleMotion>().PlayMotion();
                        }
                    }
                    else
                    {
                        UI_Elements[i].GetComponent<EMSimpleMotion>().RewindMotion();
                    }
                } catch
                {
					Debug.Log(UI_Elements[i].name + " Try Failed!");
                }
			}
		}
	}
}
