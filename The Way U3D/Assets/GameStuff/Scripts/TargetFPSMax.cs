using UnityEngine;

public class TargetFPSMax : MonoBehaviour {
	public int target = 60;
	void Start () 
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = target;
	}
}
