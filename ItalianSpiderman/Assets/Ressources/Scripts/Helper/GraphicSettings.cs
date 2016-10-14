using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class GraphicSettings : MonoBehaviour {

	private Preferences p;

	void Start () {
		p = GameObject.FindObjectOfType<Preferences>();
		gameObject.GetComponent<Bloom>().enabled = p.bloom01;
		gameObject.GetComponent<BloomOptimized>().enabled = p.bloom02;
		gameObject.GetComponent<VignetteAndChromaticAberration>().enabled = p.chroma;
		gameObject.GetComponent<NoiseAndScratches>().enabled = p.noise;
	}

}
