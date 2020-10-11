using UnityEngine;
using System.Collections;

public class ProjectileBase : MonoBehaviour
{

    public float force = 2000;
    public DoForce addForce = DoForce.AtStart;

    private Rigidbody rb;
    private AudioSource audioS;
    private bool collided = false;

    public float damage = 70;
    public float explosionForce = 1000;
    public float explosionRadius = 30;
    public AudioClip collideAudio;
    public float destroyTime = 5f;
    public bool playerImune = false;
    public bool enemyImune = false;

    public GameObject particle;
    public GameObject trail;

    public enum DoForce
    {
        AtStart,
        InFixedUpdate
    }

    // Use this for initialization
    void Start()
    {
        audioS = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        if (addForce == DoForce.AtStart)
        {
            rb.AddForce(transform.forward * force, ForceMode.Acceleration);
        }

        StartCoroutine(DestroyThis(destroyTime));
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (addForce == DoForce.InFixedUpdate)
        {
            rb.AddForce(transform.forward * force, ForceMode.Acceleration);
        }
    }
    void OnCollisionEnter()
    {
        if (!collided)
        {
            if (audioS)
            {
                audioS.PlayOneShot(collideAudio);
            }

            //get all the colliders inside the radius
            Collider[] collider = Physics.OverlapSphere(transform.position, explosionRadius);
            for (int i = 0; i < collider.Length; i++)
            {
                //
                Rigidbody r = collider[i].GetComponent<Rigidbody>();
                if (r)
                {
                    PlayerBehaviour pB = r.GetComponent<PlayerBehaviour>();
                    AIBehaviour aB = r.GetComponent<AIBehaviour>();
                    BossBehaviour bB = r.GetComponent<BossBehaviour>();
                    GiantLaserBehaviour gB = r.GetComponent<GiantLaserBehaviour>();

                    Vector3 closetPoint = collider[i].ClosestPointOnBounds(transform.position);
                    float damageDealt = damage - Vector3.Distance(transform.position, closetPoint) * (damage / explosionRadius);
                    if (pB)
                    {
                        if (!pB.invinsible & !playerImune)
                        {
                            if (damageDealt > 0)
                            {
                                pB.Damage(damageDealt);
                                if (!pB.ragdollh.ragdolled)
                                {
                                    pB.playerAnimator.SetTrigger("GotHit");
                                    if (explosionForce > 10)
                                    {
                                        pB.ToggleRagdoll();
                                        if (!pB.dead)
                                        {
                                            pB.StandUpAfter(2f);
                                        }
                                    }

                                }
                            }
                        }
                    }
                    if (!enemyImune)
                    {
                        if (aB)
                        {
                            if (damageDealt > 0)
                            {
                                aB.Damage(damageDealt);

                                aB.ToggleGotHit();
                            }
                        }

                        if (bB)
                        {
                            if (damageDealt > 0)
                            {
                                bB.Damage(damageDealt);
                            }
                        }
                        if (gB)
                        {
                            if (damageDealt > 0)
                            {
                                gB.Damage(damageDealt);
                            }
                        }

                    }
                    r.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }
            if (particle)
            {
                Instantiate(particle, transform.position, Quaternion.identity);
            }
            if (trail)
            {
                trail.transform.parent = null;
            }
            collided = true;
            gameObject.GetComponentInChildren<Renderer>().enabled = false;
            try
            {
                gameObject.GetComponentInChildren<ParticleSystem>().Stop();
            }
            catch
            {
                ;
            }
            if (audioS)
            {
                audioS.PlayOneShot(collideAudio);
            }
            StartCoroutine(DestroyThis(1f));
        }
    }

    IEnumerator DestroyThis(float time)
    {

        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
