using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Animate : MonoBehaviour
{
    [HideInInspector]
    public Animation anim;
    [HideInInspector]public bool direction = false;

    public float animationSpeed = 1;

    void Awake()
    {
        if (GetComponent<Animation>() == null)
        {
            Debug.Log("No Animation Component Found on object: " + gameObject.name);
        }
        else
        {
            anim = GetComponent<Animation>();
        }
    }

    void Start()
    {
        if (GetComponent<Animation>() == null)
        {
            Debug.Log("No Animation Component Found on object: " + gameObject.name);
        }
        else
        {
            anim = GetComponent<Animation>();
        }
    }

    public void DoAnimation()
    {
        //Debug.Log(anim.ToString());
        if (anim == null)
        {
            Debug.Log(gameObject.name + " does not have animation assigned. Trying to add one.");
            anim = GetComponent<Animation>();
        }
        else
        {
            if (!direction)
            {
                if (anim[anim.clip.name] == null)
                {
                    Debug.Log("Animation: " + anim.clip.name + " not found!!! GameObject Name: " + gameObject.name);
                }
                else
                {
                    anim[anim.clip.name].speed = animationSpeed;
                    //Debug.Log(gameObject.name);
                    //Debug.Log("Animation: " + anim.clip.name + " found!");
                }
            }
            else
            {
                anim[anim.clip.name].speed = -animationSpeed;

                //if animation already finisihed, set time to end before playing it backwards
                if (!anim.isPlaying)
                {
                    anim[anim.clip.name].time = anim[anim.clip.name].clip.length;
                }
            }

            direction = !direction;

            anim.Play(anim.clip.name);
        }
    }

    public void DoAnimation(string clipName)
    {
        //Debug.Log(anim.ToString());
        if (anim == null)
        {
            Debug.Log(gameObject.name + " does not have animation assigned. Trying to add one.");
            anim = GetComponent<Animation>();
        }
        else
        {
            if (!direction)
            {
                if (anim[clipName] == null)
                {
                    Debug.Log("Animation: " + clipName + " not found!!! GameObject Name: " + gameObject.name);
                }
                else
                {
                    anim[clipName].speed = animationSpeed;
                    //Debug.Log(gameObject.name);
                    //Debug.Log("Animation: " + anim.clip.name + " found!");
                }
            }
            else
            {
                anim[clipName].speed = -animationSpeed;

                //if animation already finisihed, set time to end before playing it backwards
                if (!anim.isPlaying)
                {
                    anim[clipName].time = anim[clipName].clip.length;
                }
            }

            direction = !direction;

            anim.Play(clipName);
        }
    }

    public void DoAnimation(float delay)
    {
        //Debug.Log(anim.ToString());
        if (anim == null)
        {
            Debug.Log(gameObject.name + " does not have animation assigned. Trying to add one.");
            anim = GetComponent<Animation>();
        }
        else
        {
            if (!direction)
            {
                if (anim[anim.clip.name] == null)
                {
                    Debug.Log("Animation: " + anim.clip.name + " not found!!! GameObject Name: " + gameObject.name);
                }
                else
                {
                    anim[anim.clip.name].speed = animationSpeed;
                    //Debug.Log(gameObject.name);
                    //Debug.Log("Animation: " + anim.clip.name + " found!");
                }
            }
            else
            {
                anim[anim.clip.name].speed = -animationSpeed;

                //if animation already finisihed, set time to end before playing it backwards
                if (!anim.isPlaying)
                {
                    anim[anim.clip.name].time = anim[anim.clip.name].clip.length;
                }
            }

            direction = !direction;
            StartCoroutine(PlayAnimationDelay(delay));
        }
    }

    private IEnumerator PlayAnimationDelay(float time)
    {
        yield return new WaitForSeconds(time);
        anim.Play(anim.clip.name);
    }
}