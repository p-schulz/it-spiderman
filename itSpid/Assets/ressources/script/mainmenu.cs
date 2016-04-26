using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class mainmenu : MonoBehaviour {

	public GameObject cam;
	public GameObject startMe;
	public GameObject schulzi;
	public GameObject fadeBlack;
	public AudioClip startClip;

	Color fade;
	float pressStart = 0.0f;
	bool started = false;
	bool pressed = false;

	IEnumerator company() {
		yield return new WaitForSeconds(1);
		schulzi.SetActive(true);
		fadeBlack.GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(1.5f);
		schulzi.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		GameObject.Find("GameState").GetComponent<GameState>().music.Play();
		yield return new WaitForSeconds(0.5f);
		cam.GetComponent<fading>().enabled = true;
	}

	void Start () {
		GameObject.Find("GameState").GetComponent<GameState>().music.clip = GameObject.Find("GameState").GetComponent<GameState>().music_intro;
		// show presenting logo
		fade.a = 1.0f;
		fade.r = 0;
		fade.g = 0;
		fade.b = 0;
		fadeBlack.GetComponent<Image>().color = fade;
		cam.GetComponent<fading>().enabled = false;
		StartCoroutine("company");
	}

	// Update is called once per frame
	void Update () {
		pressStart += Time.deltaTime;
		if(pressStart >= 12.0f && !started) {
			startMe.SetActive(true);
			started = true;
		}

		if(Input.anyKey && !pressed) {
			pressed = true;
			fadeBlack.GetComponent<AudioSource>().clip = startClip;
			fadeBlack.GetComponent<AudioSource>().Play();
			startMe.SetActive(false);
			GameObject.Find("GameState").GetComponent<GameState>().fade.FadeOutTransition(1);
		}
	}
}