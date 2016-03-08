using UnityEngine;
using System.Collections;

public class EnemyTYPE01Behaviour : MonoBehaviour {

//	float maxX = 6.1f;
//	float minX = -6.1f;
//	float maxY = 4.2f;
//	float minY = -4.2f;

	public float paddingX = 1f;
	public float paddingY = 1.5f;

	float maxX = 6.1f;
	float minX = -6.1f;
	float maxY = 4.2f;
	float minY = -1.0f;

	private float tChange = 0f; // force new direction in the first Update
	private float randomX;
	private float randomY;
	public float moveSpeed = 8.0f;
	public float tiltAngle = 30.0f;
	public float rotationSmooth = 2.0f;

	public float health = 150f;
	public int scoreValue = 150;
	public GameObject destroyParticles;
	public GameObject sparkParticles;
	public float offsetSparks = 0.5f;

	public GameObject projectile;
	public float projectileSpeed = 10f;
	public float firingRate = 0.5f;

	public AudioClip fireSound;
	public AudioClip deathSound;

	public GameObject[] Drops;

	private ScoreKeeper scoreKeeper;

	// Use this for initialization
	void Start () {
		RestrictPositionShip ();
		scoreKeeper = GameObject.Find("Score").GetComponent<ScoreKeeper>();
	}

	void RestrictPositionShip ()
	{
		float distance = transform.position.z - Camera.main.transform.position.z;
		Vector3 leftmost = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance));
		Vector3 rightmost = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distance));
		Vector3 downmost = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance));
		Vector3 upmost = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, distance));
		minX = leftmost.x + paddingX;
		maxX = rightmost.x - paddingX;
		minY = downmost.y;
		maxY = upmost.y - paddingY;
	}

	// Update is called once per frame
	void Update () {		
		// change to random direction at random intervals
		if (Time.time >= tChange) {
			randomX = Random.Range (-2.0f, 2.0f); // with float parameters, a random float
			randomY = Random.Range (-2.0f, 2.0f); //  between -2.0 and 2.0 is returned
			// set a random interval between 0.5 and 1.5
			tChange = Time.time + Random.Range (0.5f, 1.5f); 
		}

		// IMPORTANT: Change in future to improve the rotation when it goes left/right
		//*******************************************************************************
		var relativePos = transform.position;
		if (relativePos.x < randomX) {
			float tiltAroundY = tiltAngle;
			Quaternion target = Quaternion.Euler (transform.rotation.x, -tiltAroundY, transform.rotation.z);
			transform.rotation = Quaternion.Slerp (transform.rotation, target, Time.deltaTime * rotationSmooth);
			print ("Object is to the left");
		} else if (relativePos.x > randomX) {
			float tiltAroundY = -tiltAngle;
			Quaternion target = Quaternion.Euler (transform.rotation.x, -tiltAroundY, transform.rotation.z);
			transform.rotation = Quaternion.Slerp (transform.rotation, target, Time.deltaTime * rotationSmooth);
			print ("Object is to the right");
		}
		//*******************************************************************************

		transform.Translate (new Vector3 (randomX, randomY, 0f) * moveSpeed * Time.deltaTime);

		// if object reached any border, revert the appropriate direction
		if (transform.position.x >= maxX || transform.position.x <= minX) {
			randomX = -randomX;
		}
		if (transform.position.y >= maxY || transform.position.y <= minY) {
			randomY = -randomY;
		}
		// make sure the position is inside the borders
		float newX = Mathf.Clamp (transform.position.x, minX, maxX);
		float newY = Mathf.Clamp (transform.position.y, minY, maxY);
		transform.position = new Vector3 (newX, newY, transform.position.z);

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
			Instantiate (sparkParticles, new Vector3(transform.position.x,transform.position.y - offsetSparks,0), Quaternion.Euler (90, 0, 0));
			health -= missile.GetDamage();
			missile.Hit();
			if (health <= 0) {				
				Instantiate (destroyParticles, transform.position, Quaternion.identity);
				Die();
			}
		}
	}

	public float randomDrop;

	void Die(){
		AudioSource.PlayClipAtPoint(deathSound, transform.position);
		scoreKeeper.Score(scoreValue);

		if (Random.value > randomDrop) {
			Instantiate (Drops [0], new Vector3 (transform.position.x, transform.position.y, 0), Quaternion.identity);
		}

		Destroy(gameObject);
	}
}
