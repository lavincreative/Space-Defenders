/* * * * *
 * A simple Enemy TYPE01 Behaviour
 * -------------------------------
 * 
 * Written by: Alvaro Lavin 
 * 23-12-2015
 *
 * Features / attributes:
 * - 
 * - 
 * - 
 * - 
 * - 
 * - 
 * - 
 * - 
 * - 
 * 
 * 
 * 23-03-2016 Update:
 * - 
 * - 
 * - 
 * - 
 * 
 * * * * */


// ************ METHODS/FUNCTIONS ********** //
//				-----------------			 //
//											 //	
// 	  1. RESTRICT POSITION SHIP    			 //
//	  2. FIRE		     		   			 //  
// 	  3. AREA LIGTH WHEN SHOOTING  			 //  
//	  4. OnTriggerEnter2D					 //
// 	  4. DIE      				   			 //  
//   						       			 //
//								   			 //
// ***************************************** //
// ***************************************** //

using UnityEngine;
using System.Collections;

public class EnemyTYPE01Behaviour : MonoBehaviour {

//	float maxX = 6.1f;
//	float minX = -6.1f;
//	float maxY = 4.2f;
//	float minY = -4.2f;

	public float paddingX = 1f; // offset X position enemy for both sides
	public float paddingY = 1.5f; // offset Y position enemy for both sides

	float maxX = 6.1f;
	float minX = -6.1f;
	float maxY = 4.2f;
	float minY = -1.0f;

	private float tChange = 0f; // start 0 value to force new direction in the FIRST Update, this value will change through the pass of the time
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
	public GameObject lightLaserShot;
	public float lightLaserShotDuration;
	public float projectileSpeed = 10f;
	public float firingRate = 0.5f;

	public bool DieCollisionWithPlayer;

//	private GameObject mainCameraPosition;
	public AudioClip fireSound;
	public AudioClip[] impactProjectileSound;
	public AudioClip deathSound;

	public GameObject[] Drops;

	private ScoreKeeper scoreKeeper;

	void Start () {
//		mainCameraPosition = GameObject.FindGameObjectWithTag ("MainCamera");

		RestrictPositionShip ();

		scoreKeeper = GameObject.Find("Score").GetComponent<ScoreKeeper>();
	}
		
	void Update () {		
		// change to random direction at random intervals
		if (Time.time >= tChange) {
			randomX = Random.Range (-2.0f, 2.0f); // with float parameters, a random float
			randomY = Random.Range (-2.0f, 2.0f); //  between -2.0 and 2.0 is returned
			// set a random time interval between 0.5 and 1.5 to change direcion
			tChange = Time.time + Random.Range (0.5f, 1.5f); 
		}

		// IMPORTANT: Change in future to improve the rotation when it goes left/right  !!!!!!!!!!!!!!
		//*******************************************************************************
		// This is useful for a bit rotation of the enemy ship when turn left and right
		var relativePos = transform.position;
		if (relativePos.x < randomX) {
			float tiltAroundY = tiltAngle; // store enemy tilt in Y axis using the public tiltAngle variable (normal = 30.0f) = RIGHT 
			Quaternion target = Quaternion.Euler (transform.rotation.x, -tiltAroundY, transform.rotation.z); // storing right rotation in a Quaternion variable (current X axis value, new right rotation in Y axis, current Z axis value)
			transform.rotation = Quaternion.Slerp (transform.rotation, target, Time.deltaTime * rotationSmooth); // lunching the enemy right rotation
			print ("Object is to the left");
		} else if (relativePos.x > randomX) {
			float tiltAroundY = -tiltAngle; // store enemy tilt in Y axis with public tiltAngle variable (normal = -30.0f) = LEFT
			Quaternion target = Quaternion.Euler (transform.rotation.x, -tiltAroundY, transform.rotation.z); // storing left rotation in a Quaternion variable (current X axis value, new left rotation in Y axis, current Z axis value)
			transform.rotation = Quaternion.Slerp (transform.rotation, target, Time.deltaTime * rotationSmooth); // lunching the enemy left rotation
			print ("Object is to the right");
		}
		//*******************************************************************************

		transform.Translate (new Vector3 (randomX, randomY, 0f) * moveSpeed * Time.deltaTime); // moves the enemy object in the direction and distance (randomX, randomY, 0f) which it was setting at the beginning of this Update

		// if object reached any border, revert the appropriate or random direction
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

		// Randomize enemy fire between a rate and time in seconds per frame
		float prob = firingRate * Time.deltaTime;
		if (Random.value < prob) {
			Fire ();
		}
	}

	//*******************************************************************************
	//****************           RESTRICT POSITION SHIP         *********************
	//*******************************************************************************
	// Restrict position of the enemy to do not go out the screen
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

	//*******************************************************************************
	//***********************            FIRE             ***************************
	//*******************************************************************************
	void Fire() {
		StartCoroutine (SwitchAreaLigth (lightLaserShotDuration, lightLaserShot)); // active the area light each time enemy shot
		Vector3 offset = new Vector3 (0,-1,0);
		GameObject laser = Instantiate (projectile, transform.position + offset, Quaternion.identity) as GameObject;
		laser.GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, -projectileSpeed, 0); 
		AudioSource.PlayClipAtPoint (fireSound, transform.position);
//		AudioSource.PlayClipAtPoint (fireSound, new Vector3(0,0,-10));
//		AudioSource.PlayClipAtPoint (fireSound, mainCameraPosition.transform.position);
	}


	//*******************************************************************************
	//*********************     AREA LIGTH WHEN SHOOTING     ************************
	//*******************************************************************************
	private IEnumerator SwitchAreaLigth(float durationLight, GameObject light) {
		//This will turn the light on
		light.SetActive (true);

		//This will cause this function to wait for the light duration
		//which still allowing the engine execution to continue
		yield return new WaitForSeconds(durationLight);

		//This will turn the light off
		light.SetActive (false);
	}

	//*******************************************************************************
	//*******************           OnTriggerEnter2D         ************************
	//*******************************************************************************
	void OnTriggerEnter2D(Collider2D collider){
		Debug.Log (collider);
		Projectile missile = collider.gameObject.GetComponent<Projectile>();
		if(missile){
//			Instantiate (sparkParticles, new Vector3(transform.position.x,transform.position.y - offsetSparks,0), Quaternion.Euler (90, 0, 0));
			Instantiate (sparkParticles, new Vector3(collider.transform.position.x,transform.position.y - offsetSparks,0), Quaternion.Euler (90, 0, 0));
			AudioSource.PlayClipAtPoint (impactProjectileSound[Random.Range(0, impactProjectileSound.Length)], transform.position);
//			AudioSource.PlayClipAtPoint (impactProjectileSound[Random.Range(0, impactProjectileSound.Length)], new Vector3(0,0,-10));
//			AudioSource.PlayClipAtPoint (impactProjectileSound[Random.Range(0, impactProjectileSound.Length)], mainCameraPosition.transform.position);
			health -= missile.GetDamage();
			missile.Hit();
			if (health <= 0) {								
				Die();
			}
		}

		if (DieCollisionWithPlayer) {
			Die ();
		} else {
			Debug.Log ("Not detecting players:" + collider.name);
		}
	}

	public float randomPercentageDrop;


	//*******************************************************************************
	//***********************            DIE             ****************************
	//*******************************************************************************
	void Die(){
		Instantiate (destroyParticles, transform.position, Quaternion.identity);
		AudioSource.PlayClipAtPoint(deathSound, transform.position);
//		AudioSource.PlayClipAtPoint (deathSound, new Vector3(0,0,-10));
//		AudioSource.PlayClipAtPoint(deathSound, mainCameraPosition.transform.position);
		scoreKeeper.Score(scoreValue);

		if (Random.value > randomPercentageDrop) {
			Instantiate (Drops [0], new Vector3 (transform.position.x, transform.position.y, 0), Quaternion.identity);
		}
		if (Random.value > randomPercentageDrop) {
			Instantiate (Drops [1], new Vector3 (transform.position.x, transform.position.y, 0), Quaternion.identity);
		}

		Destroy(gameObject);
	}

}
