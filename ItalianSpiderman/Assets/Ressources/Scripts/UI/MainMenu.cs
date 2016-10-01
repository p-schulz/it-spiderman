using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
    public void StartNewGame()
    {
        Application.LoadLevel(1);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

	// Update is called once per frame
	void Update () {
	    
	}
}
