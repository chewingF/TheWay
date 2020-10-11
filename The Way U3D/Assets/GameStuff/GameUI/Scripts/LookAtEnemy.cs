using UnityEngine;

public class LookAtEnemy : MonoBehaviour
{
    public float distance = 200f;
    public GameObject hoverHealthBar;
	public GameObject hoverBossHealthBar;
    //private GameObject hoverEnemy;
    //private bool lookingAtTarget = false;
    //private bool actionToggle = true;

    //public GameObject hoverTextObject;
    //public GameObject gameHUDControl;
    //public GameObject soundsHUD;

    void Start()
    {
        hoverHealthBar.SetActive(false);
		hoverBossHealthBar.SetActive(false);
    }


    void Update()
    {
        int layer_mask = LayerMask.GetMask("Enemy");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            if (hit.collider.tag == "Enemy")
            {
                if (!hit.collider.gameObject.GetComponent<AIBehaviour>().dead)
                {
					hoverBossHealthBar.SetActive(false);
					hoverHealthBar.SetActive(true);
                    hoverHealthBar.GetComponent<EnemyUI>().character = hit.collider.gameObject;
                }
            }
            else if (hit.collider.tag == "Boss")
            {
                if (!hit.collider.gameObject.GetComponent<BossBehaviour>().dead)
                {
					hoverHealthBar.SetActive(false);
					hoverBossHealthBar.SetActive(true);
                    hoverBossHealthBar.GetComponent<EnemyBossUI>().character = hit.collider.gameObject;
                }
            }
            else
            {
                hoverHealthBar.SetActive(false);
			    hoverBossHealthBar.SetActive(false);
            }
        }
    }
}
