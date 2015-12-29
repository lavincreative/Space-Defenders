using UnityEngine;
using System.Collections;

public class EnemyFormationTYPE01 : MonoBehaviour {

	public GameObject enemyPrefab;
	public float width = 5f;
	public float height = 5f;
	public float speed = 5f;
	public float spawnDelay = 0.5f;
	public float padding = 1;

	// Use this for initialization
	void Start () {
		SpawnUntilFull();
	}

	void Update() {
		if(AllMembersDead()){
			Debug.Log("Empty Formation");
			SpawnUntilFull();
		}
	}

	void SpawnUntilFull(){
		Transform freePosition = NextFreePosition();
		if(freePosition){
			GameObject enemy = Instantiate(enemyPrefab, freePosition.position, Quaternion.identity) as GameObject;
			enemy.transform.parent = freePosition;
		}
		if(NextFreePosition()){
			Invoke ("SpawnUntilFull", spawnDelay);
		}
	}

	Transform NextFreePosition(){
		foreach(Transform childPositionGameObject in transform){
			if (childPositionGameObject.childCount == 0){
				return childPositionGameObject;
			}
		}
		return null;
	}

	bool AllMembersDead(){
		foreach(Transform childPositionGameObject in transform){
			if (childPositionGameObject.childCount > 0){
				return false;
			}
		}
		return true;
	}

	public void OnDrawGizmos(){
		Gizmos.DrawWireCube(transform.position, new Vector3(width, height));
	}
		
}
