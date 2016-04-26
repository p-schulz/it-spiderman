using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Director : MonoBehaviour {

	public GameObject game_state_manager;
	public GameObject player;
	public GameObject model;
	public GameObject cam;
	public GameObject fadeBlack;
	public GameObject startMe;
	public GameObject schulzi;
	public GameObject message;

	public GameState gs;
	public LevelManager lm;

	Color fade;
	float start_timer = 0.0f;
	bool started = false;
	bool pressed_start = false;
	bool prolog_loaded = false;
	bool map_loaded = false;

	IEnumerator company() {
		yield return new WaitForSeconds(1);
		schulzi.SetActive(true);
		fadeBlack.GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(1.5f);
		schulzi.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		gs.music.Play();
		yield return new WaitForSeconds(0.5f);
		gs.fade.enabled = true;
	}

	IEnumerator skipMusic() {
		yield return new WaitForSeconds(0.8f);
		message.SetActive(true);
	}

	IEnumerator loadLevel() {
		yield return new WaitForSeconds(0.2f);
		message.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		message.SetActive(false);
	}

	void SetupProlog() {
		gs.music.clip = gs.music_prolog;
		gs.music.Play();
		StartCoroutine("skipMusic");
		message = GameObject.Find("MessageProlog");
		message.SetActive(false);
	}

	void Start () {
		game_state_manager = GameObject.Find("GameState");
		cam = GameObject.Find("Main Camera");
		gs = game_state_manager.GetComponent<GameState>();
		lm = game_state_manager.GetComponent<LevelManager>();
		gs.setCurrentLevel(0);
		fade.a = 1.0f;
		fade.r = 0;
		fade.g = 0;
		fade.b = 0;
		fadeBlack.GetComponent<Image>().color = fade;
		gs.fade.enabled = false;
		gs.music.clip = gs.music_intro;
		gs.music.Stop();
		StartCoroutine("company");
		gs.writeToConsole("game started");
	}

	void Update () {
		// mainmenu
		// TODO: joints/ropes to title model, drop from ceiling (physics)
		if(gs.getCurrentLevel() == 0) {
			start_timer += Time.deltaTime;
			if(start_timer >= 12.0f && !started) {
				startMe.SetActive(true);
				started = true;
			}
			if(Input.anyKey && !pressed_start) {
				gs.writeToConsole("loading prolog...");
				pressed_start = true;
				gs.sfx.clip = gs.pass_mark;
				gs.sfx.Play();
				startMe.SetActive(false);
				gs.setNextLevel(1);
				gs.fade.FadeOutTransition(1);
			}
		}

		// prolog
		if(gs.getCurrentLevel() == 1) {
			if(!prolog_loaded) {
				pressed_start = false;
				start_timer = 0.0f;
				prolog_loaded = true;
				gs.fade.fadein = true;
				gs.fade.blend_on_start = false;
				gs.fade.ResetFade();
				SetupProlog();
			}
			if(prolog_loaded) {
				start_timer += Time.deltaTime;	
				if(start_timer > 4.0f && Input.anyKey && !pressed_start) {
					gs.writeToConsole("loading map");
					pressed_start = true;
					message.SetActive(false);
					gs.setNextLevel(2);
					gs.fade.FadeOutTransition(2);
				}
			}
		}

		// map
		if(gs.getCurrentLevel() == 2) {
            if (message == null)
                message = GameObject.Find("Start");
			if(!map_loaded) {
				message = GameObject.Find("Start");
				message.SetActive(false);
				map_loaded = true;
				gs.music.clip = gs.music_overworld;
				gs.music.Play();
				gs.fade.fadein = true;
				gs.fade.blend_on_start = false;
				gs.fade.ResetFade();
				gs.setNextLevel(3);
                started = false;
            }
			if(map_loaded) {
				if(Input.GetKeyDown(KeyCode.Return)) {
					//lm.StartLevel(3);
                    gs.writeToConsole("loading level " + gs.getCurrentLevel().ToString());
                    StartCoroutine("loadLevel");
					gs.fade.FadeOutTransition(3);
                    started = false;
				}
			}
		}

        // some level
        if(gs.getCurrentLevel() > 2 && gs.getCurrentLevel() != 5) {
            if(!started) {
                gs.levelChanged();
                gs.fade.ResetFade();
                started = true;
                map_loaded = false;
                gs.writeToConsole("start!");
            }
            //if(started) {
            //
            //}
        }

	}
}
