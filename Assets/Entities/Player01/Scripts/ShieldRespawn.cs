using UnityEngine;
using System.Collections;

public class ShieldRespawn : MonoBehaviour {

	public GameObject sparkParticles;
	public float offsetSparks = 1f;

	void OnTriggerEnter2D(Collider2D collider){
		Projectile missile = collider.gameObject.GetComponent<Projectile>();

		if (missile) {
//			Instantiate (sparkParticles, new Vector3(transform.position.x,transform.position.y - offsetSparks,0), Quaternion.Euler (-90, 0, 0));
			Instantiate (sparkParticles, new Vector3(collider.transform.position.x,transform.position.y - offsetSparks,0), Quaternion.Euler (-90, 0, 0));
			Destroy (collider.gameObject);
		} else {
			Debug.Log ("Not detecting projectil enemies:" + collider.name);
		}
	}
}
