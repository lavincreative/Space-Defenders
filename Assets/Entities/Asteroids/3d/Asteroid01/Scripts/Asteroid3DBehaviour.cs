using UnityEngine;
using System.Collections;

public class Asteroid3DBehaviour : MonoBehaviour {

	public float minTorque;
	public float maxTorque;
	public float turn;
	private float randomForwardTorque;
	private float randomUpTorque;

	public float minForceX;
	public float maxForceX;
	private float randomForce;

	private Rigidbody rigibody;

	void Start () {		
		rigibody = GetComponent<Rigidbody> ();

		randomForwardTorque = Random.Range (minTorque, maxTorque);
		randomUpTorque = Random.Range (minTorque, maxTorque);
		randomForce = Random.Range (minForceX, maxForceX);
		turn = Random.Range (-1, 1);
		while (turn <= 0.8 && turn >= -0.8) {
			turn = Random.Range (-1, 1);
		}
	}

	void Update () {

		rigibody.AddTorque (transform.forward * randomForwardTorque * turn, ForceMode.Impulse);
		rigibody.AddTorque (transform.up * randomUpTorque * turn, ForceMode.Impulse);
		rigibody.AddForce (new Vector3 (randomForce, 0, 0), ForceMode.Force);

	}
}
