/* * * * *
 * A simple Player 01 Controller
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
 * - AddComponentMenu (SpaceCraft Behaviour/Player01Controller) Quick Set from Tool Bar
 * - Editor: Headers, Ranges, Spaces (Quick View Behaviour Controllers)
 * - XInputDotNetPure library (to set Vibration gamePads)
 * - Type of fire shots through "FireTypes" enum types for several patterns shots
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
using XInputDotNetPure;

[AddComponentMenu("SpaceCraft Behaviour/Player01Controller")]
public class Player01Controller : MonoBehaviour {

	private Vector3 movementVectorAxis;
	[Header("-- Movement Settings --")]
	[Range(1, 10)]
	public float moveSpeed = 8.0f;
	public float paddingX = 1f;
	public float paddingY = 1.5f;
	[Space(10)]
	[Range(1, 10)]
	public float smoothRotation = 6.0f;
	public float tiltAngle = 30.0f;

	float xmin, xmax;
	float ymin, ymax;

	[Header("-- Projectile Settings/Behaviour --")]
	public GameObject projectile;
	public float projectileSpeed = 15.0f;
	public float firingRate = 0.2f;
	[Space(10)]
	public GameObject lightLaserShot;
	public float lightLaserShotDuration;
	[Space(10)]
	public AudioClip fireSound;

	// uncomment if we should make high volume sound effects at the camera position 
//	private GameObject mainCameraPosition;
	[Header("-- Missile Settings/Behaviour --")]
	public GameObject smallMissile;
	public float smallMissileSpeed = 15.0f;
//	public float smallMissileFiringRate = 0.0015f;
	[Tooltip("Number of Small Missiles at the beginning of the game.")]
	public int numberSmallMissile = 5;
	[Space(10)]
	public GameObject lightBeamShot;
	public float lightBeamShotDuration;
	[Space(10)]
	public AudioClip smallMissileFireSound;
	private Text numberMissiles;

	[Header("-- Particles Effect GameObjects --")]
	public GameObject deathParticles;
	public GameObject sparkParticles;
	public float offsetSparks = 0.5f;

	[Header("-- Spacecraft Settings --")]
	public float health = 250f;
	public float startDamage = 0f;
	public int lives = 3;
	[Space(10)]
	public GameObject respawnPoint;
	public GameObject shieldRespawn;
	// Collisions Variables
	[Space(10)]
	public bool DieCollisionWithEnemy;
	public bool DieCollisionWithAsteroid;
	[Space(10)]
	public PlayerHealth playerHealthHUD;
	// Health Default Slider --- Uncomment next line means uncomment all healthSlider there is in this script
	// if we wish to back to the linear bar, also we must enable Canvas >> Health Slider.
//  public Slider healthSlider;
	public Text livesText;
	private bool isDeath = false;
	private bool isCover = false;

	float speed = 4f;
	private float slowMoTimeScale;
	private float factor = 4f;
	private bool slowMoEnabled = false;

	// ACTIVATE ONLY FOR WIN - LINUX - MAC (NOT WORKING IN WEBGL)
	bool playerIndexSet = false;
	PlayerIndex playerIndex;
	GamePadState state;
	GamePadState prevState;

	private bool pauseGame = false;
	public GameObject showMenuOptions;

	LevelManager levelManager;
	private ScoreKeeper scoreKeeper;

	public enum FireTypes {
		FIRE,
		FIRETHREE,
		FIRETHREEFREE,
		FIREMULTIPLE
	}
	private FireTypes currentFire;

	//*******************************************************************************
	//****************                   START                  *********************
	//*******************************************************************************
	void Start() {	
		currentFire = FireTypes.FIRE;	
		levelManager = GameObject.Find ("LevelManager").GetComponent<LevelManager> ();
		levelManager.isPlaying = true;

		scoreKeeper = GameObject.Find ("Score").GetComponent<ScoreKeeper> ();
		// set MainCamera so AudioSource listen sounds effect in mainCamera
//		mainCameraPosition = GameObject.FindGameObjectWithTag ("MainCamera");

		// set the text object which will show the number of missiles for p01
		numberMissiles = GameObject.Find ("Number Missiles Player01").GetComponent<Text> (); 
//		livesText = GameObject.Find ("Lives").GetComponent<Text> ();

		// set the maximum p01 health into PlayerHealth maxHealth
		playerHealthHUD.maxHealth = health;
		// decrease with a start damage in p01 health if is modified with startDamage variable (0 by Default = no start damage)
		health -= startDamage;
		// set the p01 health into PlayerHealth health which will vary from maxHealth (this will take the current damage if is included before)
		playerHealthHUD.health = health;

		// set the time scale which will take the slowMo power up when is activated
//		slowMoTimeScale = Time.timeScale / factor;
		slowMoTimeScale = 1 / factor;

		// call function to restrict the position 	of the ship around the screen
		RestrictPositionShip ();
	}

	//*******************************************************************************
	//****************                  UPDATE                  *********************
	//*******************************************************************************
	void Update() {
		// ACTIVATE ONLY FOR WIN - LINUX - MAC (NOT WORKING IN WEBGL)
		// Find a PlayerIndex, for a single player game
		// Will find the first controller that is connected ans use it
		if (!playerIndexSet || !prevState.IsConnected)
		{
			for (int i = 0; i < 4; ++i)
			{
				PlayerIndex testPlayerIndex = (PlayerIndex)i;
				GamePadState testState = GamePad.GetState(testPlayerIndex);
				if (testState.IsConnected)
				{
					Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
					playerIndex = testPlayerIndex;
					playerIndexSet = true;
				}
			}
		}

		prevState = state;
		state = GamePad.GetState(playerIndex);

		//**************************************

//		healthSlider.value = health;
		// giving health % values from total to PlayerHealth attached in Canvas > HealthBarBck > HealthBar 
		// each frame to show it in the UI
		playerHealthHUD.healthBar.fillAmount = health / playerHealthHUD.maxHealth;

		if(!isDeath) {
			//*******************************************************************************
			//****************              KEYBOARD COMMANDS           *********************
			//*******************************************************************************
			if (!pauseGame) {
				// keyboard controller FIRE
				if (Input.GetKeyDown (KeyCode.L)) {
					switch (currentFire) {

					case (FireTypes.FIRE):
						InvokeRepeating ("Fire", 0.000001f, firingRate);
						break;										
					case (FireTypes.FIRETHREE):
						InvokeRepeating ("FireThree", 0.000001f, firingRate);
						break;
					case(FireTypes.FIRETHREEFREE):
						InvokeRepeating ("FireThreeFree", 0.000001f, firingRate);
						break;
					case(FireTypes.FIREMULTIPLE):
						InvokeRepeating ("FireMultiple", 0.000001f, firingRate);
						break;
						
					}
				}					
				if (Input.GetKeyUp (KeyCode.L)) {
					switch (currentFire) {

					case (FireTypes.FIRE):
						CancelInvoke ("Fire");
						break;									
					case (FireTypes.FIRETHREE):
						CancelInvoke ("FireThree");
						break;
					case(FireTypes.FIRETHREEFREE):
						CancelInvoke ("FireThreeFree");
						break;
					case(FireTypes.FIREMULTIPLE):
						CancelInvoke ("FireMultiple");
						break;

					}
				}
				// keyboard controller FIRE MISSILE
				if (Input.GetKeyDown (KeyCode.K)) {
					if (numberSmallMissile > 0) {
						StartCoroutine (SwitchAreaLigth (lightBeamShotDuration, lightBeamShot));
						FireMissile ();
					}
				}
				// slowMo power up keyboard access
				// IMPORTANT: I must improve the power up systems to give p01 limits !!!!!!!!
				if (Input.GetKeyDown (KeyCode.J)) {
					slowMoEnabled = true;
					slowMo ();
				}

			}
			if(Input.GetKeyDown(KeyCode.Escape)) {
				pauseGame = !pauseGame;

				if (pauseGame) {
					GamePad.SetVibration(playerIndex, 0, 0);
					Time.timeScale = 0;
					pauseGame = true;
					showMenuOptions.SetActive (true);
				}

				if (!pauseGame) {
					if (slowMoEnabled) {
						slowMo ();
					} else {
						Time.timeScale = 1;
					}
					pauseGame = false;
					showMenuOptions.SetActive (false);
				}
			}

			//*******************************************************************************
			//*******************************************************************************

			//*******************************************************************************
			//****************              JOYSTICK COMMANDS           *********************
			//*******************************************************************************
			if (!pauseGame) {
				// joystick controller FIRE
				if (Input.GetKeyDown (KeyCode.Joystick1Button0)) {
					switch (currentFire) {

					case (FireTypes.FIRE):
						InvokeRepeating ("Fire", 0.000001f, firingRate);
						break;										
					case (FireTypes.FIRETHREE):
						InvokeRepeating ("FireThree", 0.000001f, firingRate);
						break;
					case(FireTypes.FIRETHREEFREE):
						InvokeRepeating ("FireThreeFree", 0.000001f, firingRate);
						break;
					case(FireTypes.FIREMULTIPLE):
						InvokeRepeating ("FireMultiple", 0.000001f, firingRate);
						break;

					}
				}
				if (Input.GetKeyUp (KeyCode.Joystick1Button0)) {
					switch (currentFire) {

					case (FireTypes.FIRE):
						CancelInvoke ("Fire");
						break;									
					case (FireTypes.FIRETHREE):
						CancelInvoke ("FireThree");
						break;
					case(FireTypes.FIRETHREEFREE):
						CancelInvoke ("FireThreeFree");
						break;
					case(FireTypes.FIREMULTIPLE):
						CancelInvoke ("FireMultiple");
						break;

					}
				}
				// joystick controller FIRE MISSILE
				if (Input.GetKeyDown (KeyCode.Joystick1Button1)) {
					if (numberSmallMissile > 0) {
						StartCoroutine (SwitchAreaLigth (lightBeamShotDuration, lightBeamShot));
						FireMissile ();
					}
				}
				// slowMo power up keyboard access
				// IMPORTANT: I must improve the power up systems to give p01 limits !!!!!!!!
				if (Input.GetKeyDown (KeyCode.Joystick1Button3)) {
					slowMoEnabled = true;
					slowMo ();
				}
			}
			if(Input.GetKeyDown(KeyCode.Joystick1Button7)) {
				pauseGame = !pauseGame;

				if (pauseGame) {		
					GamePad.SetVibration(playerIndex, 0, 0);
					Time.timeScale = 0;
					pauseGame = true;
					showMenuOptions.SetActive (true);
				}

				if (!pauseGame) {
					if (slowMoEnabled) {
						slowMo ();
					} else {
						Time.timeScale = 1;
					}
					pauseGame = false;
					showMenuOptions.SetActive (false);
				}
			}
				
			//*******************************************************************************
			//*******************************************************************************

			// Smoothed movement of player ship getting Vertical and Horizontal values from Input Parameters (keyboard/joystick)
			movementVectorAxis.y = Input.GetAxisRaw ("Vertical") * moveSpeed * Time.deltaTime;
			movementVectorAxis.x = Input.GetAxisRaw ("Horizontal") * moveSpeed * Time.deltaTime;
			if (!slowMoEnabled) {
				transform.position += movementVectorAxis;
			} else {
				transform.position += movementVectorAxis * speed;
			}

			// Giving smoothed Y (horizontal) rotation when player ship goes left and right
			float tiltAroundY = Input.GetAxis("Horizontal") * tiltAngle;
			Quaternion target = Quaternion.Euler(transform.rotation.x, -tiltAroundY, transform.rotation.z);
			if (!slowMoEnabled) {
				transform.rotation = Quaternion.Slerp (transform.rotation, target, Time.deltaTime * smoothRotation);
			} else {
				transform.rotation = Quaternion.Slerp (transform.rotation, target, Time.deltaTime * smoothRotation * speed);
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

			if (scoreKeeper.score >= 5000) {
				CancelInvoke ("Fire");
				currentFire = FireTypes.FIRETHREE;
				InvokeRepeating("FireThree", 0.000001f, firingRate);
			}

			// set AudioSource to listen sound effect in mainCamera
//			AudioSource.PlayClipAtPoint (fireSound, new Vector3(0,0,-10));
//			AudioSource.PlayClipAtPoint (fireSound, mainCameraPosition.transform.position);
		}
	}

	void FireThree(){
		if (!isDeath) {
			StartCoroutine (SwitchAreaLigth (lightLaserShotDuration, lightLaserShot));
			Vector3 offset = new Vector3 (0, 1, 0);
			Vector3 offset2 = new Vector3 (0.5f, 0.5f, 0);
			Vector3 offset3 = new Vector3 (-0.5f, 0.5f, 0);
			GameObject beam = Instantiate (projectile, transform.position + offset, Quaternion.identity) as GameObject;	
			GameObject beam2 = Instantiate (projectile, transform.position + offset2, Quaternion.identity) as GameObject;	
			GameObject beam3 = Instantiate (projectile, transform.position + offset3, Quaternion.identity) as GameObject;	
			beam.GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, projectileSpeed, 0);		
			beam2.GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, projectileSpeed, 0);		
			beam3.GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, projectileSpeed, 0);		
			AudioSource.PlayClipAtPoint (fireSound, transform.position);

			if (scoreKeeper.score >= 8000) {
				CancelInvoke ("FireThree");
				currentFire = FireTypes.FIRETHREEFREE;
				InvokeRepeating("FireThreeFree", 0.000001f, firingRate);
			}

			// set AudioSource to listen sound effect in mainCamera
			//			AudioSource.PlayClipAtPoint (fireSound, new Vector3(0,0,-10));
			//			AudioSource.PlayClipAtPoint (fireSound, mainCameraPosition.transform.position);
		}
	}

	void FireThreeFree(){
		if (!isDeath) {
			StartCoroutine (SwitchAreaLigth (lightLaserShotDuration, lightLaserShot));
			Vector3 offset = new Vector3 (0, 1, 0);
			Vector3 offset2 = new Vector3 (0.5f, 0.5f, 0);
			Vector3 offset3 = new Vector3 (-0.5f, 0.5f, 0);
//			Quaternion leftLaserRot = new Quaternion (0, 0, 0);
			Quaternion laserLeftRot = Quaternion.Euler(0, 0, 10);
			Quaternion laserRightRot = Quaternion.Euler(0, 0, -10);
			GameObject beam = Instantiate (projectile, transform.position + offset, Quaternion.identity) as GameObject;	
			GameObject beam2 = Instantiate (projectile, transform.position + offset2, laserRightRot) as GameObject;	
			GameObject beam3 = Instantiate (projectile, transform.position + offset3, laserLeftRot) as GameObject;	
			beam.GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, projectileSpeed, 0);		
			beam2.GetComponent<Rigidbody2D> ().velocity = new Vector3 (5/2, projectileSpeed, 0);		
			beam3.GetComponent<Rigidbody2D> ().velocity = new Vector3 (-5/2, projectileSpeed, 0);		
			AudioSource.PlayClipAtPoint (fireSound, transform.position);

			if (scoreKeeper.score >= 10000) {
				CancelInvoke ("FireThreeFree");
				currentFire = FireTypes.FIREMULTIPLE;
				InvokeRepeating("FireMultiple", 0.000001f, firingRate);
			}

			// set AudioSource to listen sound effect in mainCamera
			//			AudioSource.PlayClipAtPoint (fireSound, new Vector3(0,0,-10));
			//			AudioSource.PlayClipAtPoint (fireSound, mainCameraPosition.transform.position);
		}
	}

	void FireMultiple(){
		if (!isDeath) {
			StartCoroutine (SwitchAreaLigth (lightLaserShotDuration, lightLaserShot));
			Vector3 offset = new Vector3 (0, 1, 0);
			Vector3 offset2 = new Vector3 (0.5f, 0.5f, 0);
			Vector3 offset3 = new Vector3 (-0.5f, 0.5f, 0);
			Vector3 offset4 = new Vector3 (1f, 0.5f, 0);
			Vector3 offset5 = new Vector3 (-1f, 0.5f, 0);
			//			Quaternion leftLaserRot = new Quaternion (0, 0, 0);
			Quaternion laserLeftRot = Quaternion.Euler(0, 0, 10);
			Quaternion laserRightRot = Quaternion.Euler(0, 0, -10);
			Quaternion laserLeftRotExt = Quaternion.Euler(0, 0, 15);
			Quaternion laserRightRotExt = Quaternion.Euler(0, 0, -15);
			GameObject beam = Instantiate (projectile, transform.position + offset, Quaternion.identity) as GameObject;	
			GameObject beam2 = Instantiate (projectile, transform.position + offset2, laserRightRot) as GameObject;	
			GameObject beam3 = Instantiate (projectile, transform.position + offset3, laserLeftRot) as GameObject;	
			GameObject beam4 = Instantiate (projectile, transform.position + offset4, laserRightRotExt) as GameObject;	
			GameObject beam5 = Instantiate (projectile, transform.position + offset5, laserLeftRotExt) as GameObject;
			beam.GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, projectileSpeed, 0);		
			beam2.GetComponent<Rigidbody2D> ().velocity = new Vector3 (5/2, projectileSpeed, 0);		
			beam3.GetComponent<Rigidbody2D> ().velocity = new Vector3 (-5/2, projectileSpeed, 0);	
			beam4.GetComponent<Rigidbody2D> ().velocity = new Vector3 (8/2, projectileSpeed, 0);		
			beam5.GetComponent<Rigidbody2D> ().velocity = new Vector3 (-8/2, projectileSpeed, 0);
			AudioSource.PlayClipAtPoint (fireSound, transform.position);

			// set AudioSource to listen sound effect in mainCamera
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
			// set AudioSource to listen sound effect in mainCamera
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

		//*******************************************************************************
		//*****************      PROJECTILES COLLISIONS SYSTEM     **********************
		//*******************************************************************************
		Projectile missile = collider.gameObject.GetComponent<Projectile>();

		if (missile && !isCover) {
			Debug.Log ("Player Collided with missile");
			StartCoroutine(GetImpactVibration(1, 0.2f)); // ACTIVATE ONLY FOR WIN - LINUX - MAC (NOT WORKING IN WEBGL)
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

		//*******************************************************************************
		//*******************      OBJECTS COLLISIONS SYSTEM     ************************
		//*******************************************************************************
		if (DieCollisionWithAsteroid && !isCover) {
			AsteroidBehaviour asteroid = collider.gameObject.GetComponent<AsteroidBehaviour> ();
			if (asteroid) {
				Debug.Log ("Player Collided with asteroid");
				health = 0;
				playerHealthHUD.health = health;
				if (health <= 0) {					
//					healthSlider.value = 0;
					playerHealthHUD.healthBar.fillAmount = 0;
					Die ();
				}
			} else {
				Debug.Log ("Not detecting asteroids:" + collider.name);
			}
		}

		if(DieCollisionWithEnemy && !isCover){
			EnemyTYPE01Behaviour enemy = collider.gameObject.GetComponent<EnemyTYPE01Behaviour> ();
			if (enemy) {
				Debug.Log ("Player Collided with enemy");
				health = 0;
				playerHealthHUD.health = health;
				if (health <= 0) {
//					healthSlider.value = 0;
					playerHealthHUD.healthBar.fillAmount = 0;
					Die ();
				}
			} else {
				Debug.Log ("Not detecting enemies:" + collider.name);
			}
		}

		//*******************************************************************************
		//***********************      TAKING DROP ITEMS     ****************************
		//*******************************************************************************
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

	// ACTIVATE ONLY FOR WIN - LINUX - MAC (NOT WORKING IN WEBGL)
	IEnumerator GetImpactVibration(float intensity, float sec) {
		GamePad.SetVibration(playerIndex, intensity, 0);
		yield return new WaitForSeconds (sec);
		GamePad.SetVibration(playerIndex, 0, 0);
	}

	//*******************************************************************************
	//***********************            DIE             ****************************
	//*******************************************************************************
	void Die(){	
		Time.timeScale = 1;
		CancelInvoke ("Fire");
		StartCoroutine(GetImpactVibration(1, 0.8f)); // ACTIVATE ONLY FOR WIN - LINUX - MAC (NOT WORKING IN WEBGL)
		Instantiate (deathParticles, transform.position, Quaternion.identity);
		lives--;
		livesText.text = lives.ToString();	
		if (lives == 0) {
			GamePad.SetVibration (playerIndex, 0, 0);
//			LevelManager man = GameObject.Find ("LevelManager").GetComponent<LevelManager> ();
//			man.LoadLevel ("Start Menu");
			levelManager.LoadLevel ("Start Menu");
			Destroy(gameObject);
		} else {	
			isDeath = true;
			isCover = true;
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
		isCover = false;
	}
		
	//*******************************************************************************
	//*******************             SLOW MOTION            ************************
	//*******************************************************************************
	void slowMo() {
		Debug.Log ("timeScale: " + Time.timeScale);
		Debug.Log ("slowMoTimeScale: " + slowMoTimeScale);
		// assing new time scale value
		Time.timeScale = slowMoTimeScale;
		Debug.Log ("timeScale: " + Time.timeScale);
		// reduce this to the same proportion as timescale to ensure smooth simulation
		Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;
		StartCoroutine(WaitRefreshSlowMo(2.0f));
	}

	IEnumerator WaitRefreshSlowMo(float waitTime) {
		yield return new WaitForSeconds (waitTime);
		// back to normal time scale
		Time.timeScale = 1;
		slowMoEnabled = false;
	}
}
