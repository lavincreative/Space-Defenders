using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour {

	public float health = 150f;
	public int scoreValue = 150;

	public GameObject projectile;
	public float projectileSpeed = 10f;
	public float firingRate = 0.5f;

	public AudioClip fireSound;
	public AudioClip deathSound;

//	private ScoreKeeper scoreKeeper;

	// Use this for initialization
	void Start () {
//		scoreKeeper = GameObject.Find ("Score").GetComponent<ScoreKeeper> ();
	}
	
	// Update is called once per frame
	void Update () {
		float prob = firingRate * Time.deltaTime;
		if (Random.value < prob) {
			Fire ();
		}
	}

	void Fire() {
		Vector3 offset = new Vector3 (0,-1,0);
		GameObject laser = Instantiate (projectile, transform.position + offset, Quaternion.identity) as GameObject;
		laser.GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, -projectileSpeed, 0); 
		AudioSource.PlayClipAtPoint (fireSound, transform.position);
	}

	void OnTriggerEnter2D(Collider2D collider){
		Debug.Log (collider);
		Projectile missile = collider.gameObject.GetComponent<Projectile>();
		if(missile){
			health -= missile.GetDamage();
			missile.Hit();
			if (health <= 0) {
				Die();
			}
		}
	}

	void Die(){
		AudioSource.PlayClipAtPoint(deathSound, transform.position);
//		scoreKeeper.Score(scoreValue);
		Destroy(gameObject);
	}
}
