using UnityEngine;
using System.Collections;

public class SuckingEffect : MonoBehaviour
{
    public float force = 2000;
    public float range = 6;
    public float ForceSpeed;
    private AIBehaviour[] aBs;
    private Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force, ForceMode.Acceleration);
        AudioManager.instance.Play("BlackHole");
    }

    public void Update()
    {
        aBs = FindObjectsOfType(typeof(AIBehaviour)) as AIBehaviour[];
        foreach (AIBehaviour aB in aBs)
        {
            float dist = Vector3.Distance(transform.position, aB.transform.position);
            if (dist < range & dist > 0)
            {
                aB.transform.position = Vector3.MoveTowards(aB.gameObject.transform.position, transform.position - transform.up,
                     ForceSpeed * Time.deltaTime);
            }
        }
    }

}