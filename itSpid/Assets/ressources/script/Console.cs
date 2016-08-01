using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Console : MonoBehaviour {

	public GameObject game_state_manager;
	public GameState status;

    public Vector2 scrollPosition;

	bool active;
	string command = "";
	string history = "";

	public void write(string s) {
		history += s + "\n";
	}

	void Start () {
		game_state_manager = GameObject.Find("GameState");
		status = game_state_manager.GetComponent<GameState>();
		history = "Italian Spiderman development build 0.7.28\n"
				+ "(c) Patrick Schulz, Jens Schindel, Fabian Gorschlüter - 2015\n"
				+ "-------------------------------------------\n"
				+ System.DateTime.Now.ToString() + "\n"
				+ SystemInfo.operatingSystem + "\n"
				+ SystemInfo.deviceType + ", " + SystemInfo.deviceModel + "\n"
				+ SystemInfo.processorType + ", " + SystemInfo.processorCount + " cores at " + SystemInfo.processorFrequency + "Hz\n"
				+ SystemInfo.graphicsDeviceName + "\n"
				+ "Graphics Shader Level: " + SystemInfo.graphicsShaderLevel + "\n"
				+ "VRAM: " + SystemInfo.graphicsMemorySize + "MB\n"
				+ "RAM: " + SystemInfo.systemMemorySize + "MB\n"
				+ "\n"
				+ "type help() for a list of commands\n";
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.C) && !active) {
			active = true;
			scrollPosition = new Vector2(0, Mathf.Infinity);
		}

		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.C) && active) {
			active = false;
		}
		
		if (Input.GetKeyDown(KeyCode.Return) && active && command != "") {
			history += "\n" + command + "\n";
			switch (command) {
				case "help":
					history += "available commands: \n"
							+ "help: list of commands \n"
							+ "time(): current time \n";
					break;
				case "time()":
					history += System.DateTime.Now.ToString() + "\n";
					break;
				default:
					history += "unknown command, type 'help' for a list of commands\n";
					break;
			}
			command = "";
			scrollPosition = new Vector2(0, Mathf.Infinity);
		}
	}

	void OnGUI() {
		if(active) {
			GUI.TextField(new Rect(0, 0, Screen.width, Screen.height / 3), "");
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height / 3));
        	GUILayout.Label(history);
        	GUILayout.EndScrollView();
			command = GUI.TextField(new Rect(0, Screen.height / 3, Screen.width, 20), command, 64);
	    }
	}
}
