using UnityEngine;
using System.Collections;

public class AsteroidBehaviour : MonoBehaviour {

	public float minTorque;
	public float maxTorque;
	private float randomTorque;
//	public float turn;
//	private float randomForwardTorque;
//	private float randomUpTorque;

	public float minForceX;
	public float maxForceX;
	private float randomForceX;

	public float minForceY;
	public float maxForceY;
	private float randomForceY;

	private Rigidbody2D rigibody;

	public float health; 
	public int scoreValue;
	public GameObject destroyParticles;
	public GameObject sparkParticles;
	public float offsetSparks = 0.5f;

	private ScoreKeeper scoreKeeper;

	void Start () {				
		rigibody = GetComponent<Rigidbody2D> ();

//		randomForwardTorque = Random.Range (minTorque, maxTorque);
//		randomUpTorque = Random.Range (minTorque, maxTorque);
		randomTorque = Random.Range(minTorque, maxTorque);
		randomForceX = Random.Range (minForceX, maxForceX);
		randomForceY = Random.Range (minForceY, maxForceY);

		scoreKeeper = GameObject.Find("Score").GetComponent<ScoreKeeper>();
	}

	void Update () {
		
		rigibody.AddTorque (randomTorque, ForceMode2D.Impulse);
//		rigibody.AddTorque (transform.up * randomUpTorque * turn, ForceMode2D.Impulse);
		rigibody.AddForce (new Vector3 (randomForceX, randomForceY, 0), ForceMode2D.Impulse);

	}

	void OnTriggerEnter2D (Collider2D collider) {
		ShieldRespawn playerShield = collider.gameObject.GetComponent<ShieldRespawn> ();
		Projectile missile = collider.gameObject.GetComponent<Projectile> ();

		if (missile) {
//			Instantiate (sparkParticles, new Vector3(transform.position.x,transform.position.y - offsetSparks,0), Quaternion.Euler (90, 0, 0));
			Instantiate (sparkParticles, new Vector3(collider.transform.position.x,transform.position.y - offsetSparks,0), Quaternion.Euler (90, 0, 0));
			health -= missile.GetDamage ();
			missile.Hit ();
			if (health <= 0) {								
				Die ();
			}
		} else {
			Debug.Log ("Not detecting projectil players:" + collider.name);
		}

		if (playerShield) {
			Die();
		}	
	}

	void Die() {
		Instantiate (destroyParticles, transform.position, Quaternion.identity);
		scoreKeeper.Score(scoreValue);
		Destroy(gameObject);
	}
}


/*
 *  FOR Rigibody3D and Collider
 *
 *
using UnityEngine;
using System.Collections;

public class AsteroidBehaviour : MonoBehaviour {

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
	}

	void Update () {
		
		rigibody.AddTorque (transform.forward * randomForwardTorque * turn, ForceMode.Impulse);
		rigibody.AddTorque (transform.up * randomUpTorque * turn, ForceMode.Impulse);
		rigibody.AddForce (new Vector3 (randomForce, 0, 0), ForceMode.Impulse);

	}
}

 * 
 * 
 * 
 * */