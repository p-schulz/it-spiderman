using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class pauseMenu : MonoBehaviour {

    public Text title;
    public Text resume;
    public Text exit;
    public Color active;
    public Color inactive;
    public bool visible;
    bool quit;


	// Use this for initialization
	void Start () {
        title.enabled = false;
        resume.enabled = false;
        exit.enabled = false;
        quit = false;
	}
	
	// Update is called once per frame
	void Update () {
	    if(visible)
        {
            title.enabled = true;
            resume.enabled = true;
            exit.enabled = true;
            if(quit)
            {
                resume.color = inactive;
                exit.color = active;
            }
            if(!quit)
            {
                resume.color = active;
                exit.color = inactive;
            }
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
                quit = !quit;
            if ((!quit && Input.GetKey(KeyCode.Return)))
            {
                visible = false;
                title.enabled = false;
                resume.enabled = false;
                exit.enabled = false;
                quit = false;
                GameObject.Find("player").GetComponent<CharController>().unpause();
            }
            if (Input.GetKey(KeyCode.Return) && quit)
            {
                GameObject.Find("GameState").GetComponent<GameState>().fade.FadeOutTransition(0);
                title.enabled = false;
                resume.enabled = false;
                exit.enabled = false;
                quit = false;
            }
        }
	}
}
