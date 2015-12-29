using UnityEngine;
using System.Collections;

public class SpawnEnemy : MonoBehaviour {

//	public GameObject enemyPrefab;

	private Transform Enemy;
	private float Timer = 10f;

	// Use this for initialization
	void Start () {
//		GameObject enemy = Instantiate (enemyPrefab, transform.position, Quaternion.identity) as GameObject;
//		enemy.transform.parent = transform;
	}

	void Awake() {
		Timer = Time.time + 10;
		Debug.Log("Awake Timer: " + Timer);
	}

	void Update() {
//		if (Timer < Time.time) { //This checks wether real time has caught up to the timer
//			Debug.Log(Timer);
//			GameObject enemy = Instantiate (enemyPrefab, transform.position, Quaternion.identity) as GameObject;
//			enemy.transform.parent = transform;
//			Timer = Time.time + 10; //This sets the timer 3 seconds into the future
//		}

//		GameObject[] enemiesArray = GameObject.FindGameObjectsWithTag ("Enemy");
//
//		foreach (GameObject enemy in enemiesArray) {
//			Debug.Log("Name of enemy: " + enemy.name);
//			Debug.Log("Number of enemies: " + enemiesArray.Length);
//		} 
//		if (enemiesArray.Length == 0) {
//			
//		}
	}

//	IEnumerator WaitForSeconds(float timeInSeconds){
//		yield return timeInSeconds;
//
//		GameObject enemy = Instantiate (enemyPrefab, transform.position, Quaternion.identity) as GameObject;
//		enemy.transform.parent = transform;
//	}

	void OnDrawGizmos(){
		Gizmos.DrawWireSphere (transform.position, 1f);
	}
}
