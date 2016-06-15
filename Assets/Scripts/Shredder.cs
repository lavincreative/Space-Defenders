using UnityEngine;
using System.Collections;

public class Shredder : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		Destroy (col.gameObject);
	}

	void OnTriggerEnter(Collider col) {
		Destroy (col.gameObject);
	}
}
