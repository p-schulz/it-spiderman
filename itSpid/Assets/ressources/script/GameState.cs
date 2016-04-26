// global game state manager,
// controls game handling and status
//
// author: schulz

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameState : MonoBehaviour {

	// global properties
	public AudioClip music_intro;
	public AudioClip music_prolog;
	public AudioClip music_plains;
	public AudioClip music_athletic;
	public AudioClip music_castle;
	public AudioClip music_underwater;
	public AudioClip music_underground;
	public AudioClip music_yoshi;
	public AudioClip music_overworld;
	public AudioClip music_finish;
	public AudioClip music_lost;
	public AudioClip music_gameover;

	public AudioClip default_jump;
	public AudioClip spin_jump;
	public AudioClip swirl;
	public AudioClip fireball;
	public AudioClip pipe;
	public AudioClip kick;
	public AudioClip pass_mark;
	public AudioClip press_pause;

	public GameObject gui_controller;
	public GameObject gui_time;
	public GameObject gui_lifes;
	public GameObject gui_coins;
	public GameObject gui_score;
	public GameObject gui_stars;
	public GameObject gui_banner;
	public fading fade;

	public AudioSource music;
	public AudioSource sfx;
	public AudioSource sfx2;

	public GameObject player;
	public GameObject model;
	public GameObject cam;

	Console console;

	bool[] levels;
	bool[] levels_cleared;
	bool[] middle_marks;
	bool[] switches_cleared;
	// switches:
	// 0 = yellow
	// 1 = green
	// 2 = red
	// 3 = blue

	bool init;

	bool running;
	bool through_pipe;
	bool paused;
	bool lost;

	int current_level;
	int next_level;
	int levels_total;
	int start;
	int intro;
	int current_map;
	int map_yoshi;
	int map_plains;

	int lifes;
	int coins;
	int score;
	int stars;
	int banner;

	int saved_status;
	int current_status;
	// status:
	// 0 = none / small
	// 1 = big
	// 2 = fire
	// 3 = cape

	// level attributes
	public float level_start;
	public float level_end = 58;
	int time_left;
	

	void Start () {
		DontDestroyOnLoad(this);
		DontDestroyOnLoad(gameObject);
		levels_total = 2;
		start = 0;
		intro = 1;
		map_yoshi = 2;
		map_plains = 3;
		current_map = 2;
		lifes = 0;
		score = 0;
		stars = 0;
		banner = 0;
		running = false;

		// dev
		//levelChanged();
		//current_level = 3;
		console = gameObject.GetComponent<Console>();
		console.write("scripts initialized");
		init = true;
		initObjects();
	}

	IEnumerator gameIsOver() {
		running = false;
		//gui_controller.SetActive(false);
		//fade.FadeOutTransition(5);
		SceneManager.LoadScene(5);
		yield return new WaitForSeconds(0.5f);
		music.clip = music_gameover;
		music.Play();
		yield return new WaitForSeconds(6);
	}

	public void writeToConsole(string s) {
		console.write(s);
	}

	public void initObjects() {
		player = GameObject.Find("player");
		model = GameObject.Find("Model");
		cam = GameObject.Find("Main Camera");
		music = GameObject.Find("Music").GetComponent<AudioSource>();
		sfx = GameObject.Find("SFX1").GetComponent<AudioSource>();
		sfx2 = GameObject.Find("SFX2").GetComponent<AudioSource>();
		fade = gameObject.GetComponent<fading>();
		gui_controller = GameObject.Find("gui_handle");
	}

	public void levelChanged() {
		
		//initObjects();
		time_left = 300;
		music.volume = 0.5f;
		lost = false;
		paused = false;
		running = true;
		music.clip = music_plains;
		music.Play();
		fade.fadeout = false;

		gui_lifes = GameObject.Find("gui_lifes");
		gui_coins = GameObject.Find("gui_coins");
		gui_score = GameObject.Find("gui_score");
		gui_time = GameObject.Find("gui_time");
		gui_stars = GameObject.Find("gui_stars");
		gui_banner = GameObject.Find("gui_banner");

	}

	public bool isGameOver() {
		return (lifes < 0) ? true : false;
	}

	public void loseLife() {
		running = false;
		console.write("player died");
		console.write("time: " + gui_time.GetComponent<Text>().text);
		console.write("remaining lifes: " + gui_lifes.GetComponent<Text>().text);
		lost = true;
		paused = true;
		changeLifes(-1);
		if (!isGameOver()) {
			setCurrentLevel(2);
			fade.FadeOutTransition(2);
		}
		if (isGameOver()) {
			//StartCoroutine("gameIsOver");
			StartCoroutine("gameIsOver");
		}
	}

	public void finishingScore() {
		score += time_left * 50;
	}

	// misc functions
	public AudioSource getMusicSource() {
		return music;
	}
	public AudioSource getSFXSource() {
		return sfx;
	}
	public void setMusicSource(AudioSource s) {
		music = s;
	}
	public void setSFXSource(AudioSource s) {
		sfx = s;
	}
	public void setMusic(AudioClip c) {
		music.clip = c;
	}
	public void setSFX(AudioClip c) {
		sfx.clip = c;
	}
	public void levelUnlocked(int i) {
		levels[i] = true;
	}
	public void levelCleared(int i) {
		if(levels[i])
			levels_cleared[i] = true;
	}
	public void switchCleared(int i) {
		switches_cleared[i] = true;
	}

	// getter
	public int getCurrentMap() {
		return current_map;
	}
	public bool getMiddelMark(int i) {
		return middle_marks[i];
	}
	public int getCurrentLevel() {
		return current_level;
	}
	public int getNextLevel() {
		return next_level;
	}
	public int getLifes() {
		return lifes;
	}
	public int getCoins() {
		return coins;
	}
	public int getScore() {
		return score;
	}
	public int getStars() {
		return stars;
	}
	public int getBannerScore() {
		return banner;
	}
	public bool getPause() {
		return paused;
	}
	public int getCurrentStatus() {
		return current_status;
	}
	public int getSavedStatus() {
		return saved_status;
	}

	// setter
    public void setRunning(bool b) {
        running = b;
    }
	public void setCurrentMap(int i) {
		current_map = i;
	}
	public void setMiddleMark(int i) {
		middle_marks[i] = true;
	}
	public void unsetMiddleMark(int i) {
		middle_marks[i] = false;
	}
	public void setCurrentLevel(int i) {
		current_level = i;
	}
	public void setNextLevel(int i) {
		next_level = i;
	}
	public void changeLifes(int i) {
		lifes += i;
	}
	public void addCoins(int i) {
		coins += i;
	}
	public void addScore(int i) {
		score += i;
	}
	public void addStars(int i) {
		stars += i;
	}
	public void addBannerScore(int i) {
		banner += i;
	}
	public void pause(bool b) {
		paused = b;
	}
	public void setCurrentStatus(int i) {
		current_status = i;
	}
	public void setSavedStatus(int i) {
		saved_status = i;
	}
	
	void Update () {

		if(init && running) {	
			// update scores
    	    gui_lifes.GetComponent<Text>().text = "x " + lifes.ToString();
			gui_coins.GetComponent<Text>().text = "x " + coins.ToString();
			gui_score.GetComponent<Text>().text = score.ToString();
			gui_stars.GetComponent<Text>().text = "x " + stars.ToString();
			gui_banner.GetComponent<Text>().text = banner.ToString();
		}		
		// update time display
		if(running && !paused) {
			time_left = (int)(301 - Time.timeSinceLevelLoad);
			gui_time.GetComponent<Text>().text = time_left.ToString();
		}

	}
}
