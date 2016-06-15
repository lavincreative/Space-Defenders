using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleFadeInEffectIntro : MonoBehaviour {

	private Image image;
	private Color color;

	public float fadeInSpeed = 1f;
	public float incrementVelocityPerFrame = 0.005f;

	void Start () {
		image = gameObject.GetComponent<Image> ();
		color.a = 0;
	}

	void Update () {
		image.color = Color.Lerp (image.color, color, fadeInSpeed * Time.deltaTime);
		fadeInSpeed = fadeInSpeed + incrementVelocityPerFrame;

		if (fadeInSpeed >= 2) Destroy(gameObject);
	}
}
