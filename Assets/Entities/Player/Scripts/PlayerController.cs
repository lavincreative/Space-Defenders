using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	private Vector3 movementVectorAxis;
	public float moveSpeed = 8.0f;
	public float paddingX = 1f;
	public float paddingY = 1f;

	public float smooth = 2.0f;
	public float tiltAngle = 30.0f;

	float xmin, xmax;
	float ymin, ymax;

	public GameObject projectile;
	public float projectileSpeed;
	public float firingRate = 0.2f;
	public AudioClip fireSound;

	public float health = 250f;
	public int lives = 3;
	public GameObject respawnPoint;
	public GameObject shieldRespawn;

	public Slider healthSlider;
	public Text livesText;

	// Use this for initialization
	void Start () {
//		livesText.text = lives.ToString();
		healthSlider.value = health;
		RestrictPositionShip ();
	}

	// Update is called once per frame
	void Update () {
		healthSlider.value = health;

		// Keyboard controller FIRE
		if (Input.GetKeyDown (KeyCode.Space)) {
			InvokeRepeating ("Fire", 0.000001f, firingRate);
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			CancelInvoke ("Fire");
		}

		// Command "XBOX" controller FIRE
		if (Input.GetKeyDown (KeyCode.Joystick1Button0)) {
			InvokeRepeating ("Fire", 0.000001f, firingRate);
		}
		if (Input.GetKeyUp (KeyCode.Joystick1Button0)) {
			CancelInvoke ("Fire");
		}

		// Smoothed movement of player ship getting Vertical and Horizontal values from Input Parameters
		movementVectorAxis.y = Input.GetAxis ("Vertical") * moveSpeed * Time.deltaTime;
		movementVectorAxis.x = Input.GetAxis ("Horizontal") * moveSpeed * Time.deltaTime;
		transform.position += movementVectorAxis;

		// Giving smoothed Y (horizontal) rotation when player ship goes left and right
		float tiltAroundY = Input.GetAxis("Horizontal") * tiltAngle;
		Quaternion target = Quaternion.Euler(transform.rotation.x, -tiltAroundY, transform.rotation.z);
		transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

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

	void Fire(){
		Vector3 offset = new Vector3 (0,1,0);
		GameObject beam = Instantiate (projectile, transform.position + offset, Quaternion.identity) as GameObject;	
		beam.GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, projectileSpeed, 0);
		AudioSource.PlayClipAtPoint(fireSound, transform.position);
	}

	void OnTriggerEnter2D(Collider2D collider){
		Projectile missile = collider.gameObject.GetComponent<Projectile>();

		if (missile) {
			Debug.Log ("Player Collided with missile");
			health -= missile.GetDamage ();
			missile.Hit ();
			if (health <= 0) {
				healthSlider.value = 0;
				Die ();
			}
		} else {
			Debug.Log ("Not detecting projectil enemies:" + collider.name);
		}


//		EnemyBehaviour enemy = collider.gameObject.GetComponent<EnemyBehaviour> ();
//		if (enemy) {
//			Debug.Log ("Player Collided with enemy");
//			health = 0;
//			if (health <= 0) {
//				Die ();
//			}
//		} else {
//			Debug.Log ("Not detecting enemies:" + collider.name);
//		}
	}

	void Die(){	
		lives--;
		livesText.text = lives.ToString();	
		if (lives == 0) {
			LevelManager man = GameObject.Find ("LevelManager").GetComponent<LevelManager> ();
			man.LoadLevel ("Start Menu");
			Destroy(gameObject);
		} else {	
			StartCoroutine(WaitRefreshLive(5.0f));
		}
	}

	IEnumerator WaitRefreshLive(float waitTime) {
		gameObject.GetComponent<MeshRenderer> ().enabled = false;
		gameObject.GetComponent<PolygonCollider2D> ().enabled = false;
//		gameObject.GetComponent<PlayerController> ().enabled = false;
		transform.position = respawnPoint.transform.position;
		yield return new WaitForSeconds (waitTime);
		health = 250f;
		healthSlider.value = health;
		gameObject.GetComponent<MeshRenderer> ().enabled = true;
		gameObject.GetComponent<PolygonCollider2D> ().enabled = true;
//		gameObject.GetComponent<PlayerController> ().enabled = true;

		shieldRespawn.SetActive (true);
		yield return new WaitForSeconds (waitTime);
		shieldRespawn.SetActive (false);
	}
		
}
