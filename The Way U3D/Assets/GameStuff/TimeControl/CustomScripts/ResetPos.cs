using UnityEngine;

public class ResetPos : MonoBehaviour 
{
	void Update () 
	{
		if (gameObject.transform.localPosition != new Vector3(0.0f, 0.0f, 0.0f))
        {
            gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }
	}
}
