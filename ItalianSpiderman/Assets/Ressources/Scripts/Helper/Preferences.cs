using UnityEngine;
using System.Collections;

public class Preferences : MonoBehaviour {

	public bool bloom01;
	public bool bloom02;
	public bool chroma;
	public bool noise;

	public void UpdateSettings(bool b1, bool b2, bool b3, bool b4) {
		bloom01 = b1;
		bloom02 = b2;
		chroma = b3;
		noise = b4;
	}

	public void bloom_old() {
		bloom01 = !bloom01;
	}

	public void bloom_new() {
		bloom02 = !bloom02;
	}

	public void chromatic() {
		chroma = !chroma;
	}

	public void noiseAndVignette() {
		noise = !noise;
	}

	void Start () {
		DontDestroyOnLoad(gameObject);
	}
}
