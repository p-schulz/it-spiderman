using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class mainmenu : MonoBehaviour {

	public GameObject gamestate;
	public GameState gs;
	public GameObject[] arrows;
	public int item = 0;

	public void changeItem(int i) {
		arrows[item].SetActive(false);
		item = Mathf.Abs((item + i) % arrows.Length);
		arrows[item].SetActive(true);
	}

	void Start() {
		gamestate = GameObject.Find("GameState");
		gs = gamestate.GetComponent<GameState>();

		for(int i = 0; i < 3; i++)
			arrows[i].SetActive(false);
		arrows[item].SetActive(true);
	}

	void Update() {
		if(Input.GetKeyDown(KeyCode.UpArrow))
			changeItem(-1);
		if(Input.GetKeyDown(KeyCode.DownArrow))
			changeItem(1);
		if(Input.GetKeyDown(KeyCode.Return) && item == 0) {
			gs.fade.FadeOutTransition(1);
		}
	}
}