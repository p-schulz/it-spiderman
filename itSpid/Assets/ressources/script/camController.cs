using UnityEngine;
using System.Collections;

public class camController : MonoBehaviour {

	public GameObject game_state_manager;
	public GameObject player;
	public GameObject model;
	public GameObject cam;
	public GameState gs;

    public float camSlopeX = 0.05f;
    public float camSlopeY = 0.01f;
    public float camPosX1 = 3.0f;
    public float camPosX2 = 2.0f;
    public float camPosY1 = 2.0f;
    public float camPosY2 = 1.0f;

	void Start () {
		game_state_manager = GameObject.Find("GameState");
		gs = game_state_manager.GetComponent<GameState>();
		player = GameObject.Find("player");
		model = GameObject.Find("Mario");
		cam = GameObject.Find("Main Camera");
	}
	
	void Update () {
		// movement on map
		if (game_state_manager.GetComponent<GameState>().getCurrentLevel() == 2 || game_state_manager.GetComponent<GameState>().getCurrentLevel() == 3) {
			gs.writeToConsole("map view");

		}

		// movement in level
		if (game_state_manager.GetComponent<GameState>().getCurrentLevel() > 3) {
			if (cam.transform.position.x > camPosX1 + model.transform.position.x)
				cam.transform.Translate(Vector3.left * (camSlopeX * Mathf.Abs(cam.transform.position.x - model.transform.position.x)));
			if (cam.transform.position.x < camPosX2 + model.transform.position.x)
				cam.transform.Translate(Vector3.right * (camSlopeX * Mathf.Abs(cam.transform.position.x - model.transform.position.x)));
			if (cam.transform.position.y > camPosY1 + model.transform.position.y)
				cam.transform.Translate(Vector3.down * (camSlopeY * Mathf.Abs(cam.transform.position.y - model.transform.position.y)));
			if (cam.transform.position.y < camPosY2 + model.transform.position.y)
				cam.transform.Translate(Vector3.up * (camSlopeY * Mathf.Abs(cam.transform.position.y - model.transform.position.y)));
		}

	}
}
