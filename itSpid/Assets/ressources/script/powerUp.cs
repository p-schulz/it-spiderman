using UnityEngine;
using System.Collections;

public class powerUp : MonoBehaviour {

	public GameObject game_state_manager;
	public bool super;
	public bool fire;
	public bool cape;

	void Start () {
		game_state_manager = GameObject.Find("GameState");
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.transform.gameObject.name == "Mario") {
			if(game_state_manager.GetComponent<GameState>().getCurrentStatus() == 0) {
				if(super)
					game_state_manager.GetComponent<GameState>().setCurrentStatus(1);
				if(fire)
					game_state_manager.GetComponent<GameState>().setCurrentStatus(2);
				if(cape)
					game_state_manager.GetComponent<GameState>().setCurrentStatus(3);
			}

			if(game_state_manager.GetComponent<GameState>().getCurrentStatus() > 0) {
				if(super)
					game_state_manager.GetComponent<GameState>().setSavedStatus(1);
				if(fire)
					game_state_manager.GetComponent<GameState>().setSavedStatus(2);
				if(cape)
					game_state_manager.GetComponent<GameState>().setSavedStatus(3);
			}
			Debug.Log("item");
			Destroy(gameObject);
		}

	}

}
