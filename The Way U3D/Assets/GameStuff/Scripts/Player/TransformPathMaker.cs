using UnityEngine;
using System.Collections;
using Chronos;

public class TransformPathMaker : BaseBehaviour {

    [HideInInspector]
    public bool play;
    [HideInInspector]
    public Transform reference;
    //private Rigidbody rb;
    [HideInInspector]
    public int state;
    [HideInInspector]
    public Vector3[] points;
    [HideInInspector]
    public float[] pointsTime;
    Vector3 correctPosition;
    

    void Start () {
        //rb = GetComponent<Rigidbody>();   
    }

	void Update () {

        if (play)
        {
            MoveTo();
        }
	}
    public void NextState()
    {
        if (state < points.Length)
        {
            state++;
            if (state < points.Length)
            {
                CorrectPosition();
            }
            else { Reset(); }
            return;
        }
    }
    void MoveTo()
    {
        if (state < points.Length)
        {
            transform.position = Vector3.Lerp(transform.position, correctPosition, pointsTime[state] * time.deltaTime);
        }
    }
    public void Reset()
    {
        time.rigidbody.isKinematic = false;
        play = false;
        state = 0;
    }
    public void Play()
    {
        if (play == false)
        {
            CorrectPosition();
            time.rigidbody.isKinematic = true;
            play = true;
            
        }
    }
    void CorrectPosition()
    {
        
        Vector3 x = reference.right * points[state].x;
        Vector3 y = new Vector3(0, points[state].y,0);
        Vector3 z = reference.forward * points[state].z;

        Vector3 toGo = reference.position + x + y + z;

        correctPosition = new Vector3(toGo.x, y.y, toGo.z);
        
        
    }
}
