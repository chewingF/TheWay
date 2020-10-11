using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using RAIN.Core;


//By Aubrey
public class AIBehaviour : MonoBehaviour
{
    [HideInInspector]
    public Animator aiAnimator;

    [Header("AI Settings")]
    [Range(1, 1000)]
    public float life = 100;
    public bool holdWeapon = true;
    public bool dead;
    public bool withoutRainAi = false;  //this is for the ai that spawn by script, which doesn't have patrol and visual sensor function

    //switch between ai drone and normal ai
    [Header("AI Drone Settings")]
    public bool drone = false;
    public GameObject droneLaser;
    private bool leftShoot = true;
    private bool rightShoot = true;


    [Header("Damage Stuff")]
    public GameObject damageNumbers;
    private GameObject newDamageNumbers;

    [Header("Enemy Soul")]
    public GameObject enemySoul;

    // Use this for initialization
    void Start()
    {
        dead = false;
        aiAnimator = GetComponent<Animator>();
        //for drone only, if without rain ai , the drone won't patrol will just follow and attack player
        if (withoutRainAi)
        {
            aiAnimator.SetBool("FoundPlayer", true);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            if (aiAnimator.GetBool("FoundPlayer"))
            {
                //remove rain ai script to stop ai patrol and visual functions
                try
                {
                    this.transform.FindDeepChild("AI").GetComponent<AIRig>().enabled = false;
                }
                catch
                {

                }
               
                if (drone)
                {
                    if (!withoutRainAi)
                    {
                        AudioManager.instance.Play("BGMusic_SecurityBreach");
                    }
                    aiAnimator.SetBool("Attack", true);
                }
                withoutRainAi = true;

                aiAnimator.SetBool("Chase", true);

                //alert near by enemies
                AIBehaviour[] aBs = FindObjectsOfType(typeof(AIBehaviour)) as AIBehaviour[];
                foreach (AIBehaviour aB in aBs)
                {
                    //if drone alert all other enemies
                    if (drone)
                    {
                        aB.transform.GetComponent<Animator>().SetBool("FoundPlayer", true);
                    }
                    else
                    {
                        float dist = Vector3.Distance(transform.position, aB.transform.position);
                        if (dist < 6 & dist > 0)
                        {
                            aB.transform.GetComponent<Animator>().SetBool("FoundPlayer", true);
                        }
                    }
                }
            }

            //normal ai without rain ai's functions
            if (withoutRainAi && !drone)
            {
                aiAnimator.SetBool("Walk", false);
                PlayerBehaviour[] pBs = FindObjectsOfType(typeof(PlayerBehaviour)) as PlayerBehaviour[];
                GameObject closeTarget = pBs[0].transform.gameObject;
                float closeDist = 999999;
                foreach (PlayerBehaviour pB in pBs)
                {
                    float dist = Vector3.Distance(transform.position, pB.transform.position);
                    if (dist < closeDist)
                    {
                        closeDist = dist;
                        closeTarget = pB.transform.gameObject;
                    }
                }
                Transform target = closeTarget.transform;
                Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPos - transform.position), 30 * Time.deltaTime);
                float distance = Vector3.Distance(transform.position, target.position);

                //Vector3 dir = (targetPos - transform.position).normalized;
                //RaycastHit hit = new RaycastHit();
                //if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
                //{
                //    if (hit.transform != transform & hit.transform != target.transform)
                //    {
                //        Debug.DrawLine(transform.position, hit.point, Color.blue);
                //        dir += hit.normal * 20;
                //    }
                //}
                //Vector3 leftR = transform.position;
                //Vector3 rightR = transform.position;

                //leftR.x -= 2;
                //rightR.x += 2;

                //if (Physics.Raycast(leftR, transform.forward, out hit, 1f))
                //{
                //    if (hit.transform != transform & hit.transform != target.transform)
                //    {
                //        Debug.DrawLine(leftR, hit.point, Color.red);
                //        dir += hit.normal * 10;
                //    }
                //}

                //if (Physics.Raycast(rightR, transform.forward, out hit, 1f))
                //{
                //    if (hit.transform != transform & hit.transform != target.transform)
                //    {
                //        Debug.DrawLine(rightR, hit.point, Color.yellow);
                //        dir += hit.normal * 10;
                //    }

                //}

                //Quaternion rot = Quaternion.LookRotation(dir);
                //transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);

                //move towards the player
                if (distance > 1.2f)
                {
                    transform.position += transform.forward * 0.2f * Time.deltaTime;
                    aiAnimator.SetBool("Attack", false);
                    aiAnimator.SetBool("Chase", true);
                }
                else
                {
                    aiAnimator.SetBool("Chase", false);
                    aiAnimator.SetBool("Attack", true);

                }

                //for normal ai, activate the weapon collider only when it is in attack animation state
                if (holdWeapon)
                {
                    if (aiAnimator.GetBool("Attack"))
                    {
                        StartCoroutine(SetWeaponCollider(true, 0.5f));
                    }
                    else
                    {
                        StartCoroutine(SetWeaponCollider(false, 0f));
                    }

                }
            }

            

            if (drone)
            {


                //check if drone should follow player, related to animator and rain ai 
                if (aiAnimator.GetBool("Chase"))
                {
                    PlayerBehaviour[] pBs = FindObjectsOfType(typeof(PlayerBehaviour)) as PlayerBehaviour[];
                    GameObject closeTarget = pBs[0].transform.gameObject;
                    float closeDist = 999999;
                    foreach (PlayerBehaviour pB in pBs)
                    {
                        float dist = Vector3.Distance(transform.position, pB.transform.position);
                        if (dist < closeDist)
                        {
                            closeDist = dist;
                            closeTarget = pB.transform.gameObject;
                        }
                    }
                    Transform target = closeTarget.transform;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position + target.up - transform.position), 30 * Time.deltaTime);
                    float distance = Vector3.Distance(transform.position, target.position);
                    //move towards the player
                    if (distance > 4)
                    {
                        transform.position += transform.forward * 5 * Time.deltaTime;
                    }
                }
                //check if drone should attack player, related to animator and rain ai 
                if (aiAnimator.GetBool("Attack"))
                {
                    if (droneLaser)
                    {
                        Transform left = transform.FindDeepChild("LeftBarrel").transform;
                        Transform right = transform.FindDeepChild("RightBarrel").transform;
                        if (left & leftShoot)
                        {
                            leftShoot = false;
                            StartCoroutine(SpawnProjectile(droneLaser, left.position + left.forward, left.rotation, 0f));
                            StartCoroutine(SetShootBool("Left", true, 0.8f));
                        }

                        if (right & rightShoot)
                        {
                            rightShoot = false;
                            StartCoroutine(SpawnProjectile(droneLaser, right.position + right.forward, right.rotation, 0f));
                            StartCoroutine(SetShootBool("Right", true, 1.2f));
                        }
                    }
                }
            }
        }
    }

    public void ToggleGotHit()
    {
        if (!dead)
        {
            aiAnimator.SetTrigger("GotHit");
            if (!aiAnimator.GetBool("Chase"))
            {
                aiAnimator.SetBool("FoundPlayer", true);
                //GameObject player = GameObject.Find("Player/Animator");
                ////create the rotation we need to be in to look at the target
                //Quaternion lookRotation = Quaternion.LookRotation((player.transform.position - transform.position).normalized);

                ////rotate us over time according to speed until we are in the required rotation
                //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 50f);
            }
        }

    }


    void OnCollisionEnter(Collision collision)
    {
        //ignore enemy weapon and normal ai collision 
        if (!drone)
        {
            if (collision.gameObject.tag == "EnemyWeapon")
            {
                Physics.IgnoreCollision(collision.collider, GetComponent<CapsuleCollider>());
            }
        }
        //player melee weapon edge collision damage
        if (collision.gameObject.tag == "Edge")
        {
            MeleeWeaponEdge e = collision.gameObject.GetComponent<MeleeWeaponEdge>();
            if (e.playerHolding && e.swinging)
            {
                Damage(e.damage);
                ToggleGotHit();
            }
        }
    }

    public void Damage(float amount)
    {
        if (!dead)
        {
            life -= amount;

            // By Nathan, Spawns the damage numbers randomly above the enemy head
            newDamageNumbers = Instantiate(damageNumbers);
            newDamageNumbers.transform.SetParent(gameObject.transform.Find("DamageNumbers"), false);
            newDamageNumbers.transform.localPosition = new Vector3(Random.Range(250.0f, -250.0f), Random.Range(-400.0f, -200.0f), 0);
            newDamageNumbers.transform.FindDeepChild("Text").GetComponent<Text>().text = Mathf.Round(amount).ToString();

            if (life <= 0)
            {
                Die();
            }
        }
    }
    public void Die()
    {
        //activate gravity for drone
        this.GetComponent<Rigidbody>().useGravity = true;
        dead = true;
        life = 0;
        if (!drone)
        {
            aiAnimator.SetTrigger("Die");
        }
        //remove rain ai script to stop ai patrol and visual functions
        try
        {
            this.transform.FindDeepChild("AI").GetComponent<AIRig>().enabled = false;
        }
        catch
        {

        }
        //switch off drone's spotlight'
        if (drone)
        {
            this.transform.FindDeepChild("Spotlight").GetComponent<Light>().enabled = false;
        }

        //disable weapon collider
        if (holdWeapon)
        {
            CapsuleCollider c = this.transform.FindDeepChild("EnemyWeapon").GetComponentInChildren<CapsuleCollider>();
            c.enabled = false;
        }

        //deactivate attack and follow
        aiAnimator.SetBool("Attack", false);
        aiAnimator.SetBool("Chase", false);

        if (!drone)
        {
            CapsuleCollider cC = aiAnimator.GetComponent<CapsuleCollider>();
            cC.direction = 2;
            cC.center = new Vector3(0, 0.3f, -1);
        }

        //Spawns the enemy soul and destroys enemy object
        if (enemySoul != null)
        {
            StartCoroutine(CreateEnemySoul(2f));
        }
        else
        {
            Debug.Log(this.gameObject.name + " is missing the enemy soul prefab! Soul not spawned!");
        }

    }

    //used to spawn projectile
    IEnumerator SpawnProjectile(GameObject p, Vector3 v, Quaternion r, float time)

    {
        yield return new WaitForSeconds(time);

        Instantiate(p, v, r);

    }
    //used to control drone shooting rate
    IEnumerator SetShootBool(string LorR, bool stat, float time)
    {
        yield return new WaitForSeconds(time);

        if (LorR == "Left")
        {
            leftShoot = stat;
        }
        else
        {
            rightShoot = stat;
        }

    }

    //used to control ai weapon collider activation time
    IEnumerator SetWeaponCollider(bool stat, float time)

    {
        yield return new WaitForSeconds(time);
        CapsuleCollider c = transform.FindDeepChild("EnemyWeapon").GetComponent<CapsuleCollider>();
        c.enabled = stat;
    }


    IEnumerator CreateEnemySoul(float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(enemySoul, this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);
    }
}