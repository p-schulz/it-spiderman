using UnityEngine;
using System.Collections;

public class CharController : MonoBehaviour {

	public GameObject game_state_manager;
	public GameObject player;
	public GameObject model;
	public GameObject cam;
	public GameState gs;

    public float rotFact = 3600.0f;
	public float speed = 0.005f;
    public bool finish;
    public bool running;

    int current_level;
    int next_level;

    bool through_pipe;
    bool paused;
    bool lost;

	void Start () {
		game_state_manager = GameObject.Find("GameState");
		player = GameObject.Find("player");
		model = GameObject.Find("Model");
		cam = GameObject.Find("Main Camera");
		gs = game_state_manager.GetComponent<GameState>();

		// dev
		lost = false;
		running = true;
	}

	IEnumerator getInPipe() {
		cam.GetComponent<SmoothCamera2D>().active = false;
        through_pipe = true;
        model.GetComponent<BoxCollider>().enabled = false;
        gs.sfx2.clip = gs.pipe;
        gs.sfx2.Play();
        yield return new WaitForSeconds(0.7f);
        //through_pipe = false;
        //gs.setCurrentLevel(2);
        gs.fade.FadeOutTransition(2);
    }
	IEnumerator lostLife() {
		cam.GetComponent<SmoothCamera2D>().active = false;	
        running = false;
        gs.setRunning(false);
		gs.music.Stop();
		gs.music.clip = gs.music_lost;
		gs.music.Play();
		model.transform.Translate(Vector3.up * Time.deltaTime);
    	yield return new WaitForSeconds(1);
    	model.GetComponent<CapsuleCollider>().enabled = false;
    	yield return new WaitForSeconds(2.5f);
		gs.loseLife();
    }
    IEnumerator finishSequence() {
        gs.music.Stop();
        gs.music.volume = 1;
        gs.music.clip = gs.music_finish;
        gs.music.Play();
        yield return new WaitForSeconds(6.0f);
        gs.fade.FadeOutTransition(2);
        running = true;
    }

	public void triggerGameOver() {
    	StartCoroutine("lostLife");
    }

	void Update () {
		paused = gs.getPause();

		// movement on map
		//if (gs.getCurrentLevel() == 2) {

		//}

		// movement in level
		if (gs.getCurrentLevel() != -1) {
			if(running && !paused && !lost && !through_pipe) {
				if (Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.D))
                    model.transform.Translate(Vector3.forward * (0.05f + speed));
				if (Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.A))
                    model.transform.Translate(Vector3.forward * (0.05f + speed));
				if (Input.GetKey(KeyCode.D))
                    model.transform.Translate(Vector3.forward * 0.05f);
                if (Input.GetKey(KeyCode.D))
                    model.transform.rotation = Quaternion.Euler(Vector3.up * 90);
                if (Input.GetKey(KeyCode.A))
                    model.transform.Translate(Vector3.forward * 0.05f);
                if (Input.GetKey(KeyCode.A))
                    model.transform.rotation = Quaternion.Euler(Vector3.up * 270);
                if (Input.GetKey(KeyCode.K))
                    model.transform.Translate(Vector3.up * 0.159f);
				if (Input.GetKey(KeyCode.S))
                    model.transform.Translate(Vector3.down * 0.1f);
				if (Input.GetKeyDown(KeyCode.K)) {
					gs.setSFX1(gs.default_jump);
					gs.sfx1.Play();
				}
				if (Input.GetKeyDown(KeyCode.L)) {
					gs.setSFX2(gs.punch);
					gs.sfx2.Play();
				}
				//if (Input.GetKey(KeyCode.L))
                //    model.transform.Rotate(Vector3.up * Time.deltaTime * rotFact);
			}

			// pause and quit (space)
			if (Input.GetKeyDown(KeyCode.P) && !lost) {
                switch (paused) { 
					case true:
						gs.setSFX2(gs.press_pause);
						gs.sfx2.Play();
						gs.getMusicSource().UnPause();
						paused = false;
						gs.writeToConsole("game resumed");
						gs.pause(false);
						Time.timeScale = 1;
						break;
					case false:
						gs.setSFX2(gs.press_pause);
						gs.sfx2.Play();
						gs.getMusicSource().Pause();
						paused = true;
						gs.writeToConsole("game paused");
						gs.pause(true);
						Time.timeScale = 0;
						break;
				}
			}
            if (Input.GetKey(KeyCode.Space) && paused && !lost) {
                gs.setSFX2(gs.press_pause);
				gs.sfx2.Play();
                Time.timeScale = 1;
                gs.fade.FadeOutTransition(2);
            }
            if (finish || Input.GetKeyDown(KeyCode.T)) {
				if(running) {
					gs.finishingScore();
					running = false;
					gs.writeToConsole("level finished");
                    gs.setRunning(false);
					gs.setCurrentLevel(2);
    	            StartCoroutine("finishSequence");
				}
			}


		}

	}
}
