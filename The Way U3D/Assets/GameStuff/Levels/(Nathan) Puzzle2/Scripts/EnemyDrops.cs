using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyDrops : MonoBehaviour
{
	// Public Variables
	[Header("Items To Drop")]
	public List<GameObject> commonItemDrops = new List<GameObject>();
	public List<GameObject> uncommonItemDrops = new List<GameObject>();
	public List<GameObject> rareItemDrops = new List<GameObject>();

	[Header("Drop Chances (%)")]
	[Range(0, 100)]
	public float common = 25f;
	[Range(0, 100)]
	public float uncommon = 10f;
	[Range(0, 100)]
	public float rare = 1f;

	[Header("Soul Despawn (Sec)")]
	public float soulDespawnTimer = 30f;

	[Header("Health Gain")]
	[Range(0, 100)]
	public float chanceToGetHealth = 50f;
	public float minHealth = 5f;
	public float maxHealth = 15f;

	[Header("Energy Gain")]
	[Range(0, 100)]
	public float chanceToGetEnergy = 75f;
	public float minEnergy = 10f;
	public float maxEnergy = 30f;

	// Private Variables
	private GameObject droppedItem;
	private PlayerBehaviour player;
	private GameObject gameManager;
	private GameObject newHealthNumbers;
	private GameObject newEnergyNumbers;

    void Start()
    {
		// Finds & Sets the Player 
		if (GameObject.Find("Player"))
			player = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
		else
			Debug.Log("Player not found for " + this.gameObject.name);

		// Finds & Sets the GameManager
		if (GameObject.Find("GameManager"))
			gameManager = GameObject.Find("GameManager");
		else
			Debug.Log("GameManager not found for " + this.gameObject.name);

		// Generates random drop chance!
		float randomChanceNum = Random.Range(0f, 100f);


		// Spawns Rare Item!
		if (randomChanceNum <= rare)
		{
			if (rareItemDrops != null)
			{
				droppedItem = Instantiate(rareItemDrops[Random.Range(0, rareItemDrops.Count - 1)], this.transform.position  + Vector3.up, this.transform.rotation);
				Debug.Log("The Rare Item " + droppedItem.name + " has Droped!");
			}
			else
			{
				Debug.Log("There are no Rare Items to Spawn!");
			}
		}
		// Spawns Uncommon Item!
		else if (randomChanceNum <= uncommon && randomChanceNum > rare)
		{
			if (uncommonItemDrops != null)
			{
				droppedItem = Instantiate(uncommonItemDrops[Random.Range(0, uncommonItemDrops.Count - 1)], this.transform.position + Vector3.up, this.transform.rotation);
				Debug.Log("The Uncommon Item " + droppedItem.name + " has Droped!");
			}
			else
			{
				Debug.Log("There are no Rare Items to Spawn!");
			}
		}
		// Spawns Common Item!
		else if (randomChanceNum <= common && randomChanceNum > uncommon)
		{
			if (commonItemDrops != null)
			{
				droppedItem = Instantiate(commonItemDrops[Random.Range(0, commonItemDrops.Count - 1)], this.transform.position + Vector3.up, this.transform.rotation);
				Debug.Log("The Common Item " + droppedItem.name + " has Droped!");
			}
			else
			{
				Debug.Log("There are no Rare Items to Spawn!");
			}
		}

		// Starts the despawn timer!
		StartCoroutine(DespawnTimer(soulDespawnTimer));
    }

	// Despawn timer
	private IEnumerator DespawnTimer(float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(this.gameObject);
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.name == "Player")
		{
			// Generates the random Health & Energy to be gained
			float healthGained = Random.Range(minHealth, maxHealth);
			float energyGained = Random.Range(minEnergy, maxEnergy);

			// Chance to get Health
			if (Random.Range(0, 100) <= chanceToGetHealth)
			{
				// Adds Health to the player
				player.life += healthGained;
				if (player.life > player.maxLife)
					player.life = player.maxLife;

				// Spawns the gained Health numbers randomly above the HealthBar
				newHealthNumbers = Instantiate(player.healthChangeNumbers.gameObject);
				newHealthNumbers.transform.SetParent(gameManager.transform.FindDeepChild("HealthBar"), false);
				newHealthNumbers.transform.localPosition = new Vector3(Random.Range(130.0f, -60.0f), Random.Range(100.0f, 130.0f), 0);
				newHealthNumbers.transform.FindDeepChild("Text").GetComponent<Text>().text = "+" + Mathf.Round(healthGained).ToString();
			}

			// Chance to get Energy
			if (Random.Range(0, 100) <= chanceToGetEnergy)
			{
				// Adds Energy to the player
				player.energy += energyGained;
				if (player.energy > player.maxEnergy)
					player.energy = player.maxEnergy;
				
				// Spawns the gained Energy numbers randomly above the EnergyBar
				newEnergyNumbers = Instantiate(player.energyChangeNumbers.gameObject);
				newEnergyNumbers.transform.SetParent(gameManager.transform.FindDeepChild("EnergyBar"), false);
				newEnergyNumbers.transform.localPosition = new Vector3(Random.Range(130.0f, -60.0f), Random.Range(100.0f, 130.0f), 0);
				newEnergyNumbers.transform.FindDeepChild("Text").GetComponent<Text>().text = "+" + Mathf.Round(energyGained).ToString();
			}
			
			// Play collect sound
			this.gameObject.transform.Find("PickUpSound").GetComponent<AudioSource>().Play();

			// Delete self
			this.GetComponent<Collider>().enabled = false;
			this.gameObject.transform.Find("Visual").gameObject.SetActive(false);
			Destroy(this.gameObject, 2f);
		}
	}
}
