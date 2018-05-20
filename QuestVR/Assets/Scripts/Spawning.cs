using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawning : MonoBehaviour {

	public GameObject[] spawingPlaces;
	public GameObject target;
	public GameObject targetCore;
	public GameObject[] mummyEnemies;
	public int maxEnemiesOnce = 4;
	public int maxEnemiesOverall = 12;
	public float spawnInterval = 2.0f;
	public float zSpawnOffset = 3.0f;

	public int currentEnemyCounter = 0;
	private int currentOverallEnemyCounter = 0;
	private float spawnTimer = 0.0f;

	void Start () {
		// 1. For each mummy enemy have to set target and targetCore
		for (int i=0; i < mummyEnemies.Length; i++) {
			mummyEnemies [i].GetComponent<MummyEnemy>().target = target;
			mummyEnemies [i].GetComponent<MummyEnemy> ().targetCore = targetCore;
			mummyEnemies [i].GetComponent<MummyEnemy> ().spawningOrigin = gameObject;
		}
	}

	void Update () {
		// Check if the total number of enemies allowed has been reached
		if (currentOverallEnemyCounter < maxEnemiesOverall) {
			spawnTimer += Time.deltaTime;
			// Check if timer interval is enough and if the max number of enemies at a point in time hasn't been reached
			if (spawnTimer % 60 > spawnInterval && currentEnemyCounter < maxEnemiesOnce) {
				spawnMummy ();
				spawnTimer = 0.0f;
			}
		}
	}

	void spawnMummy() {
		// Choose a random spawing place among the available spawning place objects
		GameObject spawningPlace = spawingPlaces [Random.Range (0, spawingPlaces.Length)];
		// Choose a random mummy to spawn from the available mummy enemy objects
		GameObject mummyEnemy = mummyEnemies [Random.Range (0, mummyEnemies.Length)];

		// Instantiate the mummyEnemy using the spawingPlace position
		Vector3 position = new Vector3(spawningPlace.transform.position.x, transform.position.y, 
			spawningPlace.transform.position.z + zSpawnOffset);
		Instantiate (mummyEnemy, position, Quaternion.identity);

		// Update enemy counters
		currentEnemyCounter += 1;
		currentOverallEnemyCounter += 1;
		print (currentEnemyCounter + " - " + currentOverallEnemyCounter);
	}
}
