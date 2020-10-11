using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollideDamage : MonoBehaviour
{
    public float damageDealt = 1;
    public float damageRate = 0.1f;
    public bool enemyImune;
    public bool playerImune;

    private bool canHit = true;

    void OnParticleCollision(GameObject other)
    {
        if (canHit)
        {
            Rigidbody r = other.GetComponent<Rigidbody>();
            if (r)
            {
                PlayerBehaviour pB = r.GetComponent<PlayerBehaviour>();
                AIBehaviour aB = r.GetComponent<AIBehaviour>();
                BossBehaviour bB = r.GetComponent<BossBehaviour>();
                GiantLaserBehaviour gB = r.GetComponent<GiantLaserBehaviour>();
                if (pB)
                {
                    if (!pB.invinsible & !playerImune)
                    {
                        pB.Damage(damageDealt);
                    }
                }
                if (!enemyImune)
                {
                    if (aB)
                    {
                        aB.Damage(damageDealt);

                        aB.ToggleGotHit();
                    }

                    if (bB)
                    {

                        bB.Damage(damageDealt);
                    }
                    if (gB)
                    {

                        gB.Damage(damageDealt);
                    }

                }
                //Vector3 direction = other.transform.position - transform.position;
                //direction = direction.normalized;
                //r.AddForce(direction * 5);
                //Destroy(gameObject);
                canHit = false;
                StartCoroutine(SetCanHitTrue());
            }
        }
    }

    IEnumerator SetCanHitTrue()
    {
        yield return new WaitForSeconds(damageRate);
        canHit = true;
    }
}
