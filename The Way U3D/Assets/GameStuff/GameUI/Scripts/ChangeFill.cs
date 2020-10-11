using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace cakeslice
{
    public class ChangeFill : MonoBehaviour
    {
        [HideInInspector]
        public GameObject player;

        [HideInInspector]
        public GameObject playButtonInfo;
        public GameObject reloadBar;
        private Image reloadBarImage;
        public GameObject rewindTimeBar;
        private Image rewindTimeBarImage;


        private float fillAmountNum;

        void Start()
        {
            player = GameObject.Find("Player");
            playButtonInfo = GameObject.Find("InputRecorder");
            reloadBarImage = reloadBar.GetComponent<Image>();
            rewindTimeBarImage = rewindTimeBar.GetComponent<Image>();
        }

        void Update()
        {
            // For Reload Bar
            if (player.GetComponent<PlayerBehaviour>().currentWeapon)
            {
                if (player.GetComponent<PlayerBehaviour>().currentWeapon.startReload)
                {
                    player.GetComponent<PlayerBehaviour>().currentWeapon.startReload = false;
                    StartCoroutine(ReloadFillImageAmount(player.GetComponent<PlayerBehaviour>().currentWeapon.reloadTime));
                }
            }

            // For Rewind Time Bar
            if (playButtonInfo.GetComponent<PlayButton>().rewindStart)
            {
                playButtonInfo.GetComponent<PlayButton>().rewindStart = false;
                rewindTimeBar.GetComponent<Animate>().DoAnimation();
            }
            if (playButtonInfo.GetComponent<PlayButton>().rewindEnd)
            {
                playButtonInfo.GetComponent<PlayButton>().rewindEnd = false;
                rewindTimeBar.GetComponent<Animate>().DoAnimation();
            }
            rewindTimeBarImage.fillAmount = playButtonInfo.GetComponent<PlayButton>().playFrom / playButtonInfo.GetComponent<PlayButton>().tempMaxFrames;
        }

        IEnumerator ReloadFillImageAmount(float reloadTime)
        {
            reloadBar.GetComponent<Animate>().DoAnimation();
            float tempTimer = 0;
            while (tempTimer < reloadTime)
            {
                tempTimer += Time.deltaTime;
                reloadBarImage.fillAmount = tempTimer / reloadTime;
                yield return null;
            }
            reloadBar.GetComponent<Animate>().DoAnimation();
        }

        // IEnumerator RewindFillImageAmount(float rewindTime)
        // {
        // 	rewindTimeBar.GetComponent<Animate>().DoAnimation();
        // 	float tempTimer = 0;
        // 	while (tempTimer < rewindTime)
        // 	{
        // 		tempTimer += Time.deltaTime;
        // 		rewindTimeBarImage.fillAmount = tempTimer/rewindTime;
        // 		yield return null;
        // 	}
        // 	rewindTimeBar.GetComponent<Animate>().DoAnimation();
        // }
    }
}