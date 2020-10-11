using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using RAIN.Core;
using RAIN.Serialization;
using RAIN.Navigation.Targets;
using RAIN.Navigation;

//By Aubrey
public class BossBehaviour : MonoBehaviour
{
    [HideInInspector]
    public Animator aiAnimator;

    [Header("Boss Settings")]
    [Range(100, 5000)]
    public float life = 500;
    public GameObject smashProjectile;
    public GameObject explodeProjectile;
    public GameObject minion;
    public GameObject chargeExplode;
    public bool dead;

    private float maxHp;
    private int explode = 2;
    private bool rage;
    private Transform bossTransform;
    private bool invincible = false;

    [Header("Damage Stuff")]
    public GameObject damageNumbers;
    private GameObject newDamageNumbers;
    private bool attacking = false;

    // Use this for initialization
    void Start()
    {
        maxHp = life;
        dead = false;
        aiAnimator = GetComponent<Animator>();
        bossTransform = aiAnimator.GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            if (!rage)
            {
                GameObject player = GameObject.Find("Player");
                Transform target = player.transform;
                Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPos - transform.position), 30 * Time.deltaTime);
                float distance = Vector3.Distance(transform.position, target.position);
                //move towards the player
                float range = Random.Range(5, 8);
                if (distance > range)
                {
                    transform.position += transform.forward * 5f * Time.deltaTime;
                    aiAnimator.SetBool("Attack", false);
                    aiAnimator.SetBool("Chase", true);
                }
                else
                {
                    aiAnimator.SetBool("Attack", true);
                    aiAnimator.SetBool("Chase", false);

                }

                //explode when hp goes below 50%
                if (life <= maxHp * 50 / 100)
                {
                    if (explode == 1)
                    {
                        invincible = true;
                        rage = true;
                        explode--;
                        aiAnimator.SetTrigger("Explode");
                        StartCoroutine(SpawnProjectile(chargeExplode, transform.position + transform.up * 3, transform.rotation, 0.5f));
                        StartCoroutine(SpawnProjectile(explodeProjectile, transform.position, transform.rotation, 2.5f));
                        StartCoroutine(SetRageBool(false, 3.5f));
                        StartCoroutine(SetInvincibleFalse(3f));
                    }
                    else
                    {
                        if (aiAnimator.GetBool("Attack"))
                        {
                            //random chance explode when hp goes below 30%
                            if (life <= maxHp * 30 / 100)
                            {
                                int chance = Random.Range(0, 5);
                                if (chance == 0)
                                {
                                    rage = true;
                                    explode--;
                                    aiAnimator.SetTrigger("Explode");
                                    StartCoroutine(SpawnProjectile(chargeExplode, transform.position + transform.up * 3, transform.rotation, 0.5f));
                                    StartCoroutine(SpawnProjectile(explodeProjectile, transform.position, transform.rotation, 2.5f));
                                    StartCoroutine(SetRageBool(false, 3.5f));                                    
                                }
                            }
                            int choice = Random.Range(0, 5);
                            switch (choice)
                            {
                                case 1 | 2:
                                    aiAnimator.SetTrigger("Kick");
                                    Transform tK = this.transform.FindDeepChild("KickProjectileSpawn");
                                    StartCoroutine(SpawnProjectile(smashProjectile, tK.position, tK.rotation, 0.55f));
                                    StartCoroutine(SpawnProjectile(smashProjectile, tK.position, tK.rotation, 0.7f));
                                    break;
                                case 4:
                                    aiAnimator.SetTrigger("Summon");
                                    StartCoroutine(SpawnProjectile(minion, transform.position + transform.forward * 3 + transform.up * 2, transform.rotation, 1f));
                                    break;
                                default:
                                    aiAnimator.SetTrigger("Smash");
                                    Transform tS = this.transform.FindDeepChild("SmashProjectileSpawn");
                                    StartCoroutine(SpawnProjectile(smashProjectile, tS.position, tS.rotation, 0.6f));
                                    StartCoroutine(SpawnProjectile(smashProjectile, tS.position + tS.up, tS.rotation, 0.8f));
                                    break;
                            }

                            rage = true;
                            StartCoroutine(SetRageBool(false, 1.5f));
                            aiAnimator.SetBool("Attack", false);

                        }
                    }
                }
                else
                {
                    //change attack pattern when hp goes below 70%
                    if (life <= maxHp * 70 / 100)
                    {
                        if (explode == 2)
                        {
                            invincible = true;
                            rage = true;
                            explode--;
                            aiAnimator.SetTrigger("Explode");
                            StartCoroutine(SpawnProjectile(chargeExplode, transform.position + transform.up * 3, transform.rotation, 0.5f));
                            StartCoroutine(SpawnProjectile(explodeProjectile, transform.position, transform.rotation, 2.5f));
                            StartCoroutine(SetRageBool(false, 4f));
                            StartCoroutine(SetInvincibleFalse(3f));
                        }
                        else
                        {
                            if (aiAnimator.GetBool("Attack"))
                            {
                                int choice = Random.Range(0, 3);
                                switch (choice)
                                {
                                    case 1:
                                        aiAnimator.SetTrigger("Kick");
                                        Transform tK = this.transform.FindDeepChild("KickProjectileSpawn");
                                        StartCoroutine(SpawnProjectile(smashProjectile, tK.position, tK.rotation, 0.55f));
                                        break;
                                    default:
                                        aiAnimator.SetTrigger("Smash");
                                        Transform tS = this.transform.FindDeepChild("SmashProjectileSpawn");
                                        StartCoroutine(SpawnProjectile(smashProjectile, tS.position, tS.rotation, 0.6f));
                                        break;

                                }
                                rage = true;
                                StartCoroutine(SetRageBool(false, 2f));
                                aiAnimator.SetBool("Attack", false);
                            }

                        }
                    }
                    else
                    {
                        if (aiAnimator.GetBool("Attack"))
                        {
                            aiAnimator.SetTrigger("Smash");
                            Transform tS = this.transform.FindDeepChild("SmashProjectileSpawn");
                            StartCoroutine(SpawnProjectile(smashProjectile, tS.position, tS.rotation, 0.6f));
                            aiAnimator.SetBool("Attack", false);
                            rage = true;
                            StartCoroutine(SetRageBool(false, 2f));
                        }
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyWeapon")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<CapsuleCollider>());
        }
        if (collision.gameObject.tag == "Edge")
        {
            MeleeWeaponEdge e = collision.gameObject.GetComponent<MeleeWeaponEdge>();
            if (e.playerHolding && e.swinging)
            {
                Damage(e.damage);

            }
        }
    }

    IEnumerator SpawnProjectile(GameObject p, Vector3 v, Quaternion r, float time)
    {
        yield return new WaitForSeconds(time);

        Instantiate(p, v, r);

    }

    IEnumerator SetRageBool(bool stat, float time)
    {
        yield return new WaitForSeconds(time);
        rage = stat;
    }


    public void Damage(float amount)
    {
        if (!dead & !invincible)
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

    IEnumerator SetInvincibleFalse(float time)
    {
        yield return new WaitForSeconds(time);
        invincible = false;
    }

    public void Die()
    {
        dead = true;
        life = 0;
        aiAnimator.SetTrigger("Die");
        CapsuleCollider cC = aiAnimator.GetComponent<CapsuleCollider>();
        cC.center = new Vector3(0, 1.1f, 0);
        cC.radius = 0.17f;
        cC.height = 1;
        cC.direction = 2;

        //if (!ragdollh.ragdolled)
        //{
        //    ToggleRagdoll();
        //}
    }
}