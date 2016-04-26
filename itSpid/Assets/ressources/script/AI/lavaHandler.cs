using UnityEngine;
using System.Collections;

public class lavaHandler : MonoBehaviour {

	public GameObject lava01;
	public GameObject lava02;
	bool playing = false;

	IEnumerator lava() {
		playing = true;
		lava01.GetComponent<AudioSource>().Play();
		lava01.GetComponent<ParticleSystem>().Play();
		lava02.GetComponent<ParticleSystem>().Play();
		yield return new WaitForSeconds(5.5f);
		playing = false;
	}

	void Update () {
		if(!playing) 
			StartCoroutine("lava");
	}
}
