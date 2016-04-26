using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public GameObject game_state_manager;
	public GameState gs;

	int next_level;
	
	void Start () {
		game_state_manager = GameObject.Find("GameState");
		gs = game_state_manager.GetComponent<GameState>();
	}
	
	public void StartLevel(int i) {
		next_level = i;
		gs.setNextLevel(next_level);
	}

	void Update () {
		
	}
}
