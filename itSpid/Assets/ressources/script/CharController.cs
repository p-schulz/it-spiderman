using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class CharController : MonoBehaviour {

	public GameObject game_state_manager;
	public GameObject player;
	public GameObject model;
	public GameObject cam;
	public GameState gs;
    public basic_behaviour last_enemy;
    public Text finished;
    public pauseMenu menu;

    public Animation anim;

    public float rotFact = 3600.0f;
    public float walkspeed = 0.045f;
	public float speed = 0.005f;
    public bool finish;
    public bool running;
    public bool punching = false;
    public bool hitting;
    public bool lost;

    int current_level;
    int next_level;

    float distToGround;

    bool paused;
    bool jumping;
    bool turning;
    bool direction; // 0 = left, 1 = right

	void Start () {
		game_state_manager = GameObject.Find("GameState");
		player = GameObject.Find("player");
		model = GameObject.Find("Model");
		cam = GameObject.Find("Main Camera");
		gs = game_state_manager.GetComponent<GameState>();
		// dev
		lost = false;
		running = true;
        distToGround = model.GetComponent<Collider>().bounds.extents.y;
    }

	IEnumerator lostLife() {
		cam.GetComponent<SmoothCamera2D>().active = false;
        last_enemy.defeated = true;
        running = false;
        gs.setRunning(false);
		//gs.music.Stop();
        //gs.music.volume = 0.55f;
        //gs.music.clip = gs.music_gameover;
	    //gs.music.Play();
		model.transform.Translate(Vector3.up * Time.deltaTime);
    	yield return new WaitForSeconds(1);
    	model.GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(5.5f);
        gs.loseLife();
        gs.fade.FadeOutTransition(0);
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

    IEnumerator jump() {
    	jumping = true;
    	yield return new WaitForSeconds(0.5f);
    	jumping = false;
    }
    IEnumerator hit() {
        hitting = true;
        punching = true;
        yield return new WaitForSeconds(0.8f);
        hitting = false;
        punching = false;
    }
    IEnumerator gotHit()
    {
        gs.setSFX2(gs.punch);
        gs.sfx2.Play();
        lost = true;
        anim["knocked_down"].speed = 1.5f;
        anim.Play("knocked_down");
        finished.text = "TERMINADO";
        yield return new WaitForSeconds(2);
        triggerGameOver();
    }
    IEnumerator turn() {
    	turning = true;
    	yield return new WaitForSeconds(0.75f);
    	turning = false;
    }
    public void unpause()
    {
        gs.pause(false);
        paused = false;
    }
    public bool getPause()
    {
        return paused;
    }
	public void triggerGameOver() {
    	StartCoroutine("lostLife");
    }

    bool isGrounded()
    {
        return Physics.Raycast(model.transform.position, -Vector3.up, distToGround + 0.1f);
    }

	void Update () {
		paused = gs.getPause();

		// movement in level
		if (gs.getCurrentLevel() != -1) {
			if(running && !paused && !lost) {

                if (!hitting && !jumping && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.K) && !Input.GetKey(KeyCode.L) && !Input.GetKey(KeyCode.I))
                    anim.Play("weight_shift");
                if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.J) && (jumping || hitting) && !turning)
                    model.transform.Translate(Vector3.forward * walkspeed);
                if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.J) && (jumping || hitting) && !turning)
                    model.transform.Translate(Vector3.forward * walkspeed);
                if (Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.D) && (jumping || hitting) && !turning)
                    model.transform.Translate(Vector3.forward * (walkspeed + speed));
                if (Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.A) && (jumping || hitting) && !turning)
                    model.transform.Translate(Vector3.forward * (walkspeed + speed));

                if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.J) && !jumping && !hitting) {
                    if(direction && !turning) {
                   		model.transform.Translate(Vector3.forward * walkspeed);
						anim.Play("walking_inPlace");
					}
					if(!direction && !jumping) {
						direction = true;
						StartCoroutine("turn");
						anim["right_turn"].speed = 2;
						anim.Play("right_turn");
					}
					//if(direction && turning)
                    //	model.transform.Translate(Vector3.back * 0.05f);
				}
                if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.J) && !jumping && !hitting) {
                    if(!direction && !turning) {
                    model.transform.Translate(Vector3.forward * walkspeed);
						anim.Play("walking_inPlace");
					}
					if(direction && !jumping) {
						direction = false;
						StartCoroutine("turn");
						anim["right_turn"].speed = 2;
						anim.Play("right_turn");
					}
					//if(!direction && turning)
                    //	model.transform.Translate(Vector3.back * 0.05f);
                }

                if (Input.GetKey(KeyCode.A) && !turning)
                    model.transform.rotation = Quaternion.Euler(Vector3.up * 270);
                if (Input.GetKey(KeyCode.D) && !turning)
                    model.transform.rotation = Quaternion.Euler(Vector3.up * 90);

                if (Input.GetKeyDown(KeyCode.K) && isGrounded())
                    model.GetComponent<Rigidbody>().AddForce(Vector3.up * 250);
                    //model.transform.Translate(Vector3.up * 0.159f);
                /*
				if (Input.GetKey(KeyCode.S)) {
                    model.transform.Translate(Vector3.down * 0.1f);
				}*/

                if (Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.D) && !jumping && !hitting && !turning) {
                    model.transform.Translate(Vector3.forward * (walkspeed + speed));
					anim.Play("running_inPlace");
				}
				if (Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.A) && !jumping && !hitting && !turning) {
                    model.transform.Translate(Vector3.forward * (walkspeed + speed));
					anim.Play("running_inPlace");
				}

				if (Input.GetKeyDown(KeyCode.K)) {
                    //jumping = true;
                    turning = false;
                    StartCoroutine("jump");
                    anim["jump"].speed = 2;
                    anim.Play("jump");
                    gs.setSFX1(gs.default_jump);
					gs.sfx1.Play();
				}
				if (Input.GetKeyDown(KeyCode.L) && !jumping && !hitting) {
                    //StartCoroutine("punchopuncho");
					gs.setSFX2(gs.punch);
					gs.sfx2.Play();
                    StartCoroutine("hit");
                    anim["kicking"].speed = 2;
					anim.Play("kicking");
				}
				if (Input.GetKeyDown(KeyCode.I) && !jumping && !hitting) {
                    //StartCoroutine("punchopuncho");
					gs.setSFX2(gs.punch);
					gs.sfx2.Play();
                    StartCoroutine("hit");
					anim.Play("uppercut");
				}
                if (Input.GetKeyDown(KeyCode.Escape) && !menu.visible && !lost)
                {
                    gs.pause(true);
                    paused = true;
                    menu.visible = true;
                }
			}
            /*
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
            */
            if (Input.GetKey(KeyCode.Space) && paused && !lost) {
                gs.setSFX2(gs.press_pause);
				gs.sfx2.Play();
                Time.timeScale = 1;
                gs.fade.FadeOutTransition(0);
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
