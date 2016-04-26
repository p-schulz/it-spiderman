using UnityEngine;
using System.Collections;

public class setLevel : MonoBehaviour {
	
	public GameObject cam;
	public int levelNumber = 3;
	public Material activeMat;
	public Material notactiveMat;

	// set current index
	void OnMouseDown() {
		gameObject.GetComponent<AudioSource>().Play();
		cam.GetComponent<map>().setLevelIndex(levelNumber);
		GameObject.Find("gamestate MASTER").GetComponent<gameStateManager>().setNextLevel(levelNumber);
		//Debug.Log("currentLvl = " + GameObject.Find("gamestate MASTER").GetComponent<gameStateManager>().getCurrentLevel());
		//Debug.Log("nextLvl = " + GameObject.Find("gamestate MASTER").GetComponent<gameStateManager>().getNextLevel());
	}

	// Update is called once per frame
	void Update () {
		if (levelNumber == cam.GetComponent<map>().currentLevel()) {
			Renderer rend = GetComponent<Renderer>();
			rend.material = activeMat;
		}

		if (levelNumber != cam.GetComponent<map>().currentLevel()) {
			Renderer rend = GetComponent<Renderer>();
			rend.material = notactiveMat;
		}
	}
}
