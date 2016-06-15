using UnityEngine;
using System.Collections;

public class DespawnExplosionSparks : MonoBehaviour {

	public float delaySeconds;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Destroy (gameObject, delaySeconds);
	}
}
