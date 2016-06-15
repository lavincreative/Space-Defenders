/* * * * *
 * A simple Player 02 Controller
 * -----------------------------
 * 
 * Written by: Alvaro Lavin 
 * 23-12-2015
 *
 * Features / attributes:
 * - Simple Fire & Missile Fire System -> Keyboard & Joystick commands
 * - Smooth spacecraft movement x/y axis + smooth horizontal rotation left/right moves
 * - Movement screen restriction in x/y axis depending of screen size
 * - Fire with 2D asset laser beam + area light when shooting + fire sound -> keep press fire button to invoke repeating fire / repeating key down/up faster than just pressed
 * - Missile Fire 3D sphere + particles beam effect + area light when shooting + missile fire sound
 * - Trigger Collider Systems
 * 		- With projectiles
 * 		- With asteroids -> bool
 * 		- With enemies -> bool
 * 		- Missile Drop Pack
 * 		- Helath Drop Pack
 * - Death system
 * - Power up system
 * 		- Slow Motion (In process, need improvement)
 * 		- Energy Shield (ToDo)
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
//	  0. START								 //
//	  1. UPDATE								 //
// 	  3. RESTRICT POSITION SHIP    			 //
//	  4. FIRE		     		   			 //  
//	  5. FIRE MISSILE    		   			 //  
// 	  6. AREA LIGTH WHEN SHOOTING  			 //  
//	  7. OnTriggerEnter2D					 //
// 	  8. DIE      				   			 //
//    9. WaitRefreshLive 					 //
//   						       			 //
//								   			 //
// ***************************************** //
// ***************************************** //

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player02Controller : MonoBehaviour {

	private Vector3 movementVectorAxis;
	public float moveSpeed = 8.0f;
	public float paddingX = 1f;
	public float paddingY = 1.5f;

	public float smooth = 2.0f;
	public float tiltAngle = 30.0f;

	float xmin, xmax;
	float ymin, ymax;

	public GameObject projectile;
	public GameObject lightLaserShot;
	public float lightLaserShotDuration;
	public float projectileSpeed;
	public float firingRate = 0.2f;
	public AudioClip fireSound;

//	private GameObject mainCameraPosition;
	public GameObject smallMissile;
	public GameObject lightBeamShot;
	public float lightBeamShotDuration;
	public float smallMissileSpeed;
	public float smallMissileFiringRate = 0.2f;
	public AudioClip smallMissileFireSound;
	public int numberSmallMissile = 5;
	private Text numberMissiles;

	public bool DieCollisionWithEnemy;
	public GameObject destroyParticles;
	public GameObject sparkParticles;
	public float offsetSparks = 0.5f;
	public float health = 250f;
	public float startDamage = 0f;
	public int lives = 3;
	public GameObject respawnPoint;
	public GameObject shieldRespawn;

	public bool CollisionWithAsteroid;

	public PlayerHealth playerHealthHUD;
	// Health Default Slider --- Uncomment next line means uncomment all healthSlider are in this script
	// if we wish to back to the linear bar, also we must enable Canvas >> Health Slider.
	//	public Slider healthSlider;
	public Text livesText;
	private bool isDeath = false;

	float speed = 4f;
	private float slowMoTimeScale;
	private float factor = 4f;

	void Start () {
//		mainCameraPosition = GameObject.FindGameObjectWithTag ("MainCamera");

		numberMissiles = GameObject.Find ("Number Missiles Player02").GetComponent<Text> ();

		playerHealthHUD.maxHealth = health;
		health -= startDamage;
		playerHealthHUD.health = health;

		slowMoTimeScale = Time.timeScale / factor;

		RestrictPositionShip ();
	}

	void slowMo() {
		// assing new time scale value
		Time.timeScale = slowMoTimeScale;
		// reduce this to the same proportion as timescale to ensure smooth simulation
		Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;
		StartCoroutine(WaitRefreshLiveTwo(2.0f));
	}

	IEnumerator WaitRefreshLiveTwo(float waitTime) {
		yield return new WaitForSeconds (waitTime);
		Time.timeScale = 1;
		slowMoEnabled = false;
	}

	private bool slowMoEnabled = false;
	void Update () {
		//		healthSlider.value = health;
		playerHealthHUD.healthBar.fillAmount = health / playerHealthHUD.maxHealth;

		// slowMo access
		if(Input.GetKeyDown (KeyCode.LeftControl)) {
			slowMoEnabled = true;
			slowMo();
		}
		if (Input.GetKeyDown (KeyCode.Joystick1Button3)) {
			slowMoEnabled = true;
			slowMo();
		}

		if(!isDeath) {
			// Keyboard controller FIRE
			if (Input.GetKeyDown (KeyCode.Space)) {
				InvokeRepeating ("Fire", 0.000001f, firingRate);
			}
			if (Input.GetKeyUp (KeyCode.Space)) {
				CancelInvoke ("Fire");
			}
			if (Input.GetKeyDown (KeyCode.LeftAlt)) {
				if(numberSmallMissile > 0) {
					StartCoroutine (SwitchAreaLigth (lightBeamShotDuration, lightBeamShot));
					FireMissile ();
					//				InvokeRepeating ("FireMissile", 0.000001f, firingRate);
				}
			}

			// Command "XBOX" controller FIRE
			if (Input.GetKeyDown (KeyCode.Joystick2Button0)) {
				InvokeRepeating ("Fire", 0.000001f, firingRate);
			}
			if (Input.GetKeyUp (KeyCode.Joystick2Button0)) {
				CancelInvoke ("Fire");
			}
			if (Input.GetKeyDown (KeyCode.Joystick2Button1)) {
				if (numberSmallMissile > 0) {
					StartCoroutine (SwitchAreaLigth (lightBeamShotDuration, lightBeamShot));
					FireMissile ();
				}
			}

			// Smoothed movement of player ship getting Vertical and Horizontal values from Input Parameters
			movementVectorAxis.y = Input.GetAxisRaw ("Player02_Vertical") * moveSpeed * Time.deltaTime;
			movementVectorAxis.x = Input.GetAxisRaw ("Player02_Horizontal") * moveSpeed * Time.deltaTime;
			if (!slowMoEnabled) {
				transform.position += movementVectorAxis;
			} else {
				transform.position += movementVectorAxis * speed;
			}

			// Giving smoothed Y (horizontal) rotation when player ship goes left and right
			float tiltAroundY = Input.GetAxis("Player02_Horizontal") * tiltAngle;
			Quaternion target = Quaternion.Euler(transform.rotation.x, -tiltAroundY, transform.rotation.z);
			if (!slowMoEnabled) {
				transform.rotation = Quaternion.Slerp (transform.rotation, target, Time.deltaTime * smooth);
			} else {
				transform.rotation = Quaternion.Slerp (transform.rotation, target, Time.deltaTime * smooth * speed);
			}

			// Restricting the player's position to gamespace (Starting with RestrictPositionShip() method) 
			float newX = Mathf.Clamp (transform.position.x, xmin, xmax);
			float newY = Mathf.Clamp (transform.position.y, ymin, ymax);
			transform.position = new Vector3 (newX, newY, transform.position.z);

			//		// Arrow keys from keyboard
			//		if (Input.GetKey (KeyCode.LeftArrow)) {			
			////			transform.position += new Vector3 (-moveSpeed * Time.deltaTime, 0, 0);
			//			transform.position += Vector3.left * moveSpeed * Time.deltaTime;
			//		} else if (Input.GetKey(KeyCode.RightArrow)) {			
			////			transform.position += new Vector3 (moveSpeed * Time.deltaTime, 0, 0);
			//			transform.position += Vector3.right * moveSpeed * Time.deltaTime;
			//		}
			//
			//		if (Input.GetKey (KeyCode.UpArrow)) {			
			////			transform.position += new Vector3 (0, moveSpeed * Time.deltaTime, 0);
			//			transform.position += Vector3.up * moveSpeed * Time.deltaTime;	
			//		} else if (Input.GetKey (KeyCode.DownArrow)) {			
			////			transform.position += new Vector3 (0, -moveSpeed * Time.deltaTime, 0);
			//			transform.position += Vector3.down * moveSpeed * Time.deltaTime;
			//		}
		}
	}

	//*******************************************************************************
	//****************           RESTRICT POSITION SHIP         *********************
	//*******************************************************************************
	void RestrictPositionShip ()
	{
		float distance = transform.position.z - Camera.main.transform.position.z;
		Vector3 leftmost = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance));
		Vector3 rightmost = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distance));
		Vector3 downmost = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance));
		Vector3 upmost = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, distance));
		xmin = leftmost.x + paddingX;
		xmax = rightmost.x - paddingX;
		ymin = downmost.y;
		ymax = upmost.y - paddingY;
	}

	//*******************************************************************************
	//***********************            FIRE             ***************************
	//*******************************************************************************
	void Fire(){
		if (!isDeath) {
			StartCoroutine (SwitchAreaLigth (lightLaserShotDuration, lightLaserShot));
			Vector3 offset = new Vector3 (0, 1, 0);
			GameObject beam = Instantiate (projectile, transform.position + offset, Quaternion.identity) as GameObject;	
			beam.GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, projectileSpeed, 0);
			AudioSource.PlayClipAtPoint (fireSound, transform.position);
			//			AudioSource.PlayClipAtPoint (fireSound, new Vector3(0,0,-10));
			//			AudioSource.PlayClipAtPoint (fireSound, mainCameraPosition.transform.position);
		}
	}

	//*******************************************************************************
	//***********************         FIRE MISSILE        ***************************
	//*******************************************************************************
	void FireMissile(){
		if (!isDeath) {					
			Vector3 offset = new Vector3 (0, 1, 0);
			GameObject beam = Instantiate (smallMissile, transform.position + offset, Quaternion.identity) as GameObject;	
			beam.GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, smallMissileSpeed, 0);
			AudioSource.PlayClipAtPoint (smallMissileFireSound, transform.position);	
			//			AudioSource.PlayClipAtPoint (smallMissileFireSound, new Vector3(0,0,-10));
			//			AudioSource.PlayClipAtPoint (smallMissileFireSound, mainCameraPosition.transform.position);
			numberSmallMissile--;
			numberMissiles.text = numberSmallMissile.ToString();
		}
	}

	//*******************************************************************************
	//*********************     AREA LIGTH WHEN SHOOTING     ************************
	//*******************************************************************************
	private IEnumerator SwitchAreaLigth(float durationLight, GameObject light) {
		//This will turn the light on
		light.SetActive (true);

		//This will cause this function to wait for the light duration
		//whilst still allowing the engine execution to continue
		yield return new WaitForSeconds(durationLight);

		//This will turn the light off
		light.SetActive (false);
	}


	//*******************************************************************************
	//*******************           OnTriggerEnter2D         ************************
	//*******************************************************************************
	void OnTriggerEnter2D(Collider2D collider){
		Projectile missile = collider.gameObject.GetComponent<Projectile>();

		if (missile) {
			Debug.Log ("Player Collided with missile");
			//			Instantiate (sparkParticles, new Vector3(transform.position.x,transform.position.y - offsetSparks,0), Quaternion.Euler (90, 0, 0));
			Instantiate (sparkParticles, new Vector3(collider.transform.position.x,transform.position.y - offsetSparks,0), Quaternion.Euler (-90, 0, 0));
			health -= missile.GetDamage ();
			playerHealthHUD.health = health;
			missile.Hit ();
			if (health <= 0) {
				//				healthSlider.value = 0;
				playerHealthHUD.healthBar.fillAmount = 0;
				Die ();
			}
		} else {
			Debug.Log ("Not detecting projectil enemies:" + collider.name);
		}

		if (CollisionWithAsteroid) {
			AsteroidBehaviour asteroid = collider.gameObject.GetComponent<AsteroidBehaviour> ();
			if (asteroid) {
				Debug.Log ("Player Collided with asteroid");
				health = 0;
				if (health <= 0) {					
					//					healthSlider.value = 0;
					playerHealthHUD.healthBar.fillAmount = 0;
					Die ();
				}
			} else {
				Debug.Log ("Not detecting asteroids:" + collider.name);
			}
		}

		if(DieCollisionWithEnemy){
			EnemyTYPE01Behaviour enemy = collider.gameObject.GetComponent<EnemyTYPE01Behaviour> ();
			if (enemy) {
				Debug.Log ("Player Collided with enemy");
				health = 0;
				if (health <= 0) {
					//					healthSlider.value = 0;
					playerHealthHUD.healthBar.fillAmount = 0;
					Die ();
				}
			} else {
				Debug.Log ("Not detecting enemies:" + collider.name);
			}
		}

		if (collider.tag == "SmallMissilePack") 
		{			
			numberSmallMissile = numberSmallMissile + 5;
			numberMissiles.text = numberSmallMissile.ToString();
			Destroy (collider.gameObject);
		}

		if (collider.tag == "HealthPack") 
		{
			Debug.Log ("HealthPack");
			if (health <= 150f) {
				health += 100f;
				//				healthSlider.value = health;
				playerHealthHUD.health = health;
				Destroy (collider.gameObject);
			}
		}
	}

	//*******************************************************************************
	//***********************            DIE             ****************************
	//*******************************************************************************
	void Die(){	
		Instantiate (destroyParticles, transform.position, Quaternion.identity);
		lives--;
		livesText.text = lives.ToString();	
		if (lives == 0) {
			LevelManager man = GameObject.Find ("LevelManager").GetComponent<LevelManager> ();
			man.LoadLevel ("Start Menu");
			Destroy(gameObject);
		} else {	
			isDeath = true;
			StartCoroutine(WaitRefreshLive(5.0f));
		}
	}

	//*******************************************************************************
	//*******************           WaitRefreshLive          ************************
	//*******************************************************************************
	IEnumerator WaitRefreshLive(float waitTime) {
		gameObject.GetComponent<MeshRenderer> ().enabled = false;
		gameObject.GetComponent<PolygonCollider2D> ().enabled = false;
		//		gameObject.GetComponent<PlayerController> ().enabled = false;
		transform.position = respawnPoint.transform.position;
		yield return new WaitForSeconds (waitTime);
		health = 250f;
		//		healthSlider.value = health;
		playerHealthHUD.health = health;
		gameObject.GetComponent<MeshRenderer> ().enabled = true;
		gameObject.GetComponent<PolygonCollider2D> ().enabled = true;
		//		gameObject.GetComponent<PlayerController> ().enabled = true;
		isDeath = false;
		shieldRespawn.SetActive (true);
		yield return new WaitForSeconds (waitTime);
		shieldRespawn.SetActive (false);
	}

}
