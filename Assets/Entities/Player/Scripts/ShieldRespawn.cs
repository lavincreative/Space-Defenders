using UnityEngine;
using System.Collections;

public class ShieldRespawn : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider){
		Projectile missile = collider.gameObject.GetComponent<Projectile>();

		if (missile) {
			Destroy (collider.gameObject);
		} else {
			Debug.Log ("Not detecting projectil enemies:" + collider.name);
		}
	}
}
