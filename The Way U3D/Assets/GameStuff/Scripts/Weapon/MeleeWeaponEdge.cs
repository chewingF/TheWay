using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponEdge : MonoBehaviour
{
    [Header("Edge Attributes Settings")]
    [Range(0, 100)]
    public float damage = 30;
    public GameObject collideParticlePrefab;

    [HideInInspector]
    public bool playerHolding;
    public bool swinging;
    public AudioClip collideEnemies;
    public AudioClip collideObstacles;
    private BoxCollider bc;
    private AudioSource audioS;

    // Use this for initialization
    void Start()
    {
        playerHolding = false;
        swinging = false;
        bc = GetComponent<BoxCollider>();
        audioS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        //ignore collision with player
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Bone")
        {
            Physics.IgnoreCollision(collision.collider, bc);
        } 
        else
        {
            if (playerHolding && swinging)
            {
                if (collision.gameObject.tag == "Enemy")
                {
                    audioS.PlayOneShot(collideEnemies);
                }
                foreach (ContactPoint contact in collision.contacts)
                {
                    
                    //Instantiate your particle system here.
                    Instantiate(collideParticlePrefab, contact.point, Quaternion.identity);
                    //add force
                    Rigidbody r = collision.gameObject.GetComponent<Rigidbody>();
                    if (r)
                    {
                        r.AddExplosionForce(30, transform.position, 0.5f);
                    }
                }
            }
        }
       
    }

    IEnumerator SetSwingfalseAfter(float time)
    {
        yield return new WaitForSeconds(time);

        swinging = false;
    }

    public void CoroutineSetSwingfalseAfter(float time)
    {
        StartCoroutine(SetSwingfalseAfter(time));
    }

    public void SetHolding(bool stat)
    {
        playerHolding = stat;
    }

    public void SetSwinging(bool stat)
    {
        swinging = stat;
    }
}