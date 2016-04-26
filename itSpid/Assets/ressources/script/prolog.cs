using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class prolog : MonoBehaviour {
	
	public GameObject message;
	public GameObject cam;
	float musicFinished = 0.0f;

	IEnumerator skipMusic() {
		cam.GetComponent<AudioSource>().volume = 0;
		yield return new WaitForSeconds(0.8f);
		cam.GetComponent<AudioSource>().volume = 1;
	}

	void Start () {
		// workaround for audio clip
		StartCoroutine("skipMusic");
	}

	void Update () {
		// onetime functions
		musicFinished += Time.deltaTime;	
		if(musicFinished > 4.0f && Input.anyKey) {
			message.SetActive(false);
			cam.GetComponent<fading>().FadeOut();
		}
	}
}