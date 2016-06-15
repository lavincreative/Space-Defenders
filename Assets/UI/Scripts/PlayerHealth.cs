using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
	
	public float maxHealth;
	public float health;
	public Image healthBar;

	void Update () {
		gameObject.GetComponent<Image> ().fillAmount = health / maxHealth;
	}
}
