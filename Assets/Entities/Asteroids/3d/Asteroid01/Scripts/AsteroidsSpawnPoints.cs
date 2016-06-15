using UnityEngine;
using System.Collections;

public class AsteroidsSpawnPoints : MonoBehaviour {

	public GameObject AsteroidPrefab;
	public float spawnTime = 3f;  
	public Transform[] spawnPoints;

	void Start () {
		// Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
		InvokeRepeating ("Spawn", spawnTime, spawnTime);
	}

	void Update () {
	
	}

	void Spawn() {
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);
//		Instantiate (AsteroidPrefab, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
		Instantiate (AsteroidPrefab, spawnPoints[spawnPointIndex].position, Quaternion.identity);
	}
}
