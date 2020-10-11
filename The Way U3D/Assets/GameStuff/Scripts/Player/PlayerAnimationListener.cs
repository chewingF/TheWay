using UnityEngine;
using System.Collections;

public class PlayerAnimationListener : MonoBehaviour {

    private TransformPathMaker pathMaker;
    private PlayerBehaviour playerBehaviour;
    private Vector3 defailtPos = new Vector3(0,0,0);

    void Start()
    {
        pathMaker = transform.parent.GetComponent<TransformPathMaker>();
        playerBehaviour = pathMaker.gameObject.GetComponent<PlayerBehaviour>();  
    }
    void Update()
    {
        if (this.transform.localPosition != defailtPos)
        {
            this.transform.localPosition = defailtPos;
        }
    }
    public void PlayPathMaker()
    {
        pathMaker.Play();
    }
    public void ResetPathMaker()
    {
        pathMaker.Reset();
    }
	public void NextClimbState()
    {
        pathMaker.NextState();
    }
    public void Jump()
    {
        playerBehaviour.Jump();
    }
}
