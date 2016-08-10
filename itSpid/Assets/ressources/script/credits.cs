using UnityEngine;
using System.Collections;

public class credits : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
    IEnumerator creditsTimer()
    {
        yield return new WaitForSeconds(75);
        GameObject.Find("GameState").GetComponent<fading>().FadeOutTransition(0);

        // TRAILER PURPOSES:
        /*
        yield return new WaitForSeconds(65);
        GameObject.Find("GameState").GetComponent<fading>().FadeOutTransition(4);
        yield return new WaitForSeconds(35);
        GameObject.Find("GameState").GetComponent<fading>().FadeOutTransition(5);
        */
    }

	// Update is called once per frame
	void Update () {

        StartCoroutine("creditsTimer");
	}
}
