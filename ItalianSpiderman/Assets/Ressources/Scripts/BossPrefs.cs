using UnityEngine;
using System.Collections;

public class BossPrefs : MonoBehaviour {

	public GameObject Bounding;
	public GameObject Boss;
	public AudioSource defaultMusic;
	public GameObject finishLine;

	public int nextLevel = 0;
	public bool gameFinished = false;
	public bool endFight = false;

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.name == "Player" && !gameFinished && !endFight) {
			Bounding.SetActive(true);
			Boss.SetActive(true);
			endFight = true;
		}
	}

	IEnumerator Leave()
	{
		defaultMusic.Stop();
		finishLine.SetActive(true);
		gameObject.GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(5);
		GameObject.FindObjectOfType<GameMaster>().SetNextLevel(nextLevel);
		GameObject.FindObjectOfType<GameMaster>().GameOver();
	}

	// Update is called once per frame
	void Update () {
		if(endFight && !gameFinished)
			if(Boss == null) {
				gameFinished = true;
				StartCoroutine("Leave");
			}
	}
}
