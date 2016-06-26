// script for fading in from black and 
// fading out to black incl. music fading.
//
// author: schulzi

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class fading : MonoBehaviour {

	public GameObject fade_black;
	public GameObject game_state_manager;
	public GameState gs;
	public Color alpha_fade;

	public int next_level;
    public float fade_in_delay = 0.5f;
    public float fade_in_speed = 0.03f;
	public float fade_out_speed = 0.1f;

	public bool blend_on_start;
    public bool fadein = true;
	public bool fadeout = false;

    bool in_delay = true;
    bool delay_finished = false;


	void Start () {
		game_state_manager = GameObject.Find("GameState");
		gs = game_state_manager.GetComponent<GameState>();
		fade_black = GameObject.Find("fader");
		DontDestroyOnLoad(fade_black);

		if(blend_on_start) {
			alpha_fade.a = 0.0f; 
			fade_black.GetComponent<Image>().color = alpha_fade;
		}
		if(fadein) {
			alpha_fade.a = 1.0f;
			fade_black.GetComponent<Image>().color = alpha_fade;
		}
	}

    IEnumerator FadeInDelay()
    {
        in_delay = false;
        gs.writeToConsole("Delay for fade-in: " + fade_in_delay + " seconds");
        yield return new WaitForSeconds(fade_in_delay);
        delay_finished = true;
        gs.writeToConsole("Fading done!");
    }

	public void ResetFade() {
		game_state_manager = GameObject.Find("GameState");
		gs = game_state_manager.GetComponent<GameState>();
		fade_black = GameObject.Find("fader");
		gs.music.volume = 1;
		fadeout = false;
		if(blend_on_start) {
			alpha_fade.a = 0.0f; 
			fade_black.GetComponent<Image>().color = alpha_fade;
		}
		if(fadein) {
			alpha_fade.a = 1.0f;
			fade_black.GetComponent<Image>().color = alpha_fade;
        }
	}

	public void FadeOut() {
		fadeout = true;
	}

    public void FadeOutTransition(int i)
    {
        next_level = i;
        fadeout = true;
    }

	void Update () {

        if(!delay_finished && in_delay)
            StartCoroutine(FadeInDelay());

        if (fadein && alpha_fade.a > 0 && delay_finished) {
			alpha_fade.a -= fade_in_speed;
			fade_black.GetComponent<Image>().color = alpha_fade;
		}

		if(fadein && alpha_fade.a <= 0)
			fadein = false;

		if(fadeout && gs.music.volume > 0) {
			gs.music.volume -= 0.005f;
			alpha_fade.a += fade_out_speed;
			fade_black.GetComponent<Image>().color = alpha_fade;
		}

		if(fadeout && gs.music.volume <= 0) {
			gs.music.Stop();
			gs.setCurrentLevel(next_level);
			SceneManager.LoadScene(next_level);
			gs.initObjects();
			fadeout = false;
		}
	}
}