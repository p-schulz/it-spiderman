using UnityEngine;
using System.Collections;

public class SmoothCamera2D : MonoBehaviour {

	public Transform target;
	public Camera camera;
	public float damp_time = 0.15f;
	public float offset_x = 0.5f;
	public float offset_y = 2;
	public bool active = false;
	public bool shift = false;

	private Vector3 velocity = Vector3.zero;
	//Vector3 point;
	//Vector3 delta;
	//Vector3 destination;
	int current_level;

	void Start() {
		current_level = GameObject.Find("GameState").GetComponent<GameState>().getCurrentLevel();
	}

	void Update()  {
		if (target && active) { // && current_level > 3
			Vector3 point = camera.WorldToViewportPoint(target.position);
			Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(offset_x, offset_y, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, damp_time);
		}
		if (Input.GetKeyDown(KeyCode.Q) && offset_x >= 0) {
			offset_x -= Time.deltaTime * 0.1f;
			GameObject.Find("GameState").GetComponent<Console>().write("camera shift left");
		}
		if (Input.GetKeyDown(KeyCode.E) && offset_x <= 1) {
			offset_x += Time.deltaTime * 0.1f;
			GameObject.Find("GameState").GetComponent<Console>().write("camera shift right");
		}
	}
}