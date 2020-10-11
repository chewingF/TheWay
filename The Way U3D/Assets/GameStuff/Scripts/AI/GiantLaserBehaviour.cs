using System.Collections;
using UnityEngine;
using RAIN.Core;


//By Aubrey
public class GiantLaserBehaviour : MonoBehaviour
{
    [Header("Giant Laser Settings")]
    [Range(100, 10000)]
    public float life = 3000;
    [Range(1, 20)]
    public float targetScaleMultiplier = 10;
    [Range(1f, 600)]
    public float chargingTime = 60f;
    public GameObject shootObject;
    public GameObject explosion;
    public GameObject minion;
    public float summonRate = 20;

    private GameObject chargeObject1;
    private GameObject chargeObject2;
    private GameObject chargeObject3;
    private Vector3 baseScale;
    private bool dead;
    private bool stop;
    private bool summon = true;
    
    // Use this for initialization
    void Start()
    {
        dead = false;
        chargeObject1 = transform.FindDeepChild("charge1").gameObject;
        chargeObject2 = transform.FindDeepChild("charge2").gameObject;
        chargeObject3 = transform.FindDeepChild("charge3").gameObject;
        //Get only one of them, since all of their base scale are the same
        baseScale = chargeObject1.transform.localScale;
        stop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            stop = true;
            StartCoroutine(StopParticles(0f));
           
        } else
        {
            if (summon)
            {
                summon = false;
                Instantiate(minion, transform.position +transform.forward* Random.Range(-3, 3) + transform.up*20, transform.rotation);
                StartCoroutine(SetSummonTrue());
            }
        }
        if (!stop)
        {
            //increase the size of particles effect(charging)
            float t = 1/ chargingTime;
            chargeObject1.transform.localScale = Vector3.MoveTowards(chargeObject1.transform.localScale, baseScale * targetScaleMultiplier, t);
            chargeObject2.transform.localScale = Vector3.MoveTowards(chargeObject2.transform.localScale, baseScale * targetScaleMultiplier, t);
            chargeObject3.transform.localScale = Vector3.MoveTowards(chargeObject3.transform.localScale, baseScale * targetScaleMultiplier, t);

            //if the charging size reach target scale, kill player
            if (chargeObject1.transform.localScale == baseScale * targetScaleMultiplier)
            {
                Quaternion FacingRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y +180, transform.rotation.z);
                Instantiate(shootObject, transform.position + transform.up * 21 - transform.right * 23, FacingRotation);
                stop = true;
                StartCoroutine(StopParticles(5f));
                StartCoroutine(KillPlayer(0.3f));
            }
        }
    }


    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Edge")
        {
            MeleeWeaponEdge e = collision.gameObject.GetComponent<MeleeWeaponEdge>();
            if (e.playerHolding && e.swinging)
            {
                Damage(e.damage);

            }
        }
    }


    public void Damage(float amount)
    {
        if (!dead)
        {
            life -= amount;
            if (life <= 0)
            {
                Die();
            }
        }
    }
    public void Die()
    {
        dead = true;
        life = 0;
        StartCoroutine(Explosion());
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        StartCoroutine(DestroySelf(4));
    }

    IEnumerator StopParticles(float time)
    {
        yield return new WaitForSeconds(time);
        transform.Find("Charge").gameObject.SetActive(false);
    }

    IEnumerator KillPlayer(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject player = GameObject.Find("Player");
        player.GetComponent<PlayerBehaviour>().Damage(100000);
    }
    //used to control summon rate
    IEnumerator SetSummonTrue()
    {
        yield return new WaitForSeconds(summonRate);
        summon = true;
    }
    IEnumerator Explosion()
    {
        Instantiate(explosion, transform.position + transform.up * 30 - transform.right * 10 + transform.forward * 3, transform.rotation);
        yield return new WaitForSeconds(0.3f);
        Instantiate(explosion, transform.position + transform.up * 27 - transform.right * 13 + transform.forward * 6, transform.rotation);
        yield return new WaitForSeconds(0.5f);
        Instantiate(explosion, transform.position + transform.up * 25 - transform.right * 11 + transform.forward * -4, transform.rotation);
        yield return new WaitForSeconds(0.4f);
        Instantiate(explosion, transform.position + transform.up * 28 - transform.right * 10 + transform.forward * 2, transform.rotation);
        yield return new WaitForSeconds(0.3f);
        Instantiate(explosion, transform.position + transform.up * 24 - transform.right * 15 + transform.forward *-7, transform.rotation);
    }

    IEnumerator DestroySelf(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyObject(this);
    }
}