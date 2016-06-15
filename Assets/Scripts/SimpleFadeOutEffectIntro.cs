using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleFadeOutEffectIntro : MonoBehaviour {

	private Image image;
	private Color color;

	public float fadeOutSpeed = 1f;
	public float incrementVelocityPerFrame = 0.005f;

	void Start () {
		image = gameObject.GetComponent<Image> ();
		color.a = 1;
	}

	void Update () {
		image.color = Color.Lerp (image.color, color, fadeOutSpeed * Time.deltaTime);
		fadeOutSpeed = fadeOutSpeed + incrementVelocityPerFrame;
	}
}
