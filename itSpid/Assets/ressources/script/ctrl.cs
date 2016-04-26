using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ctrl : MonoBehaviour {

	public GameObject player;
	public GameObject model;
	public GameObject cam;
	public GameObject timeDisplayed;
	public GameObject lifesObject;
	public GameObject coinsObject;
	public GameObject scoreObject;
	public GameObject starsObject;
	public GameObject bannerScoreObject;
	public GameObject master;
	public GameObject sfx;
	public GameObject backFade;

    public Color black;

	public AudioClip jump;
	public AudioClip spinjump;
    public AudioClip finish;
    public AudioClip pipe;
    public AudioClip gameOver;

    public int levelEnd = 28;
	public int timeLeft = 300;
	public int currentLvl;

    public float rotFact = 3600.0f;
	public float speed = 0.005f;
    public float lockPosX = 90.0f;
    public float lockPosZ = 0.0f;
    public float camSlopeX = 0.05f;
    public float camSlopeY = 0.01f;
    public float camPosX1 = 3.0f;
    public float camPosX2 = 2.0f;
    public float camPosY1 = 2.0f;
    public float camPosY2 = 1.0f;
    public float[] pipes_x;

    bool paused = false;
    bool throughPipe = false;
	bool running = true;
	bool lost = false;

	void Start () {
		// set gamestate manager
		master = GameObject.Find("gamestate MASTER");
		currentLvl = master.GetComponent<gameStateManager>().getCurrentLevel();
    }
    
    IEnumerator finishSequence() {
        black.a = 1;
        backFade.GetComponent<SpriteRenderer>().color = black;
        cam.GetComponent<AudioSource>().Stop();
        cam.GetComponent<AudioSource>().volume = 1;
        cam.GetComponent<AudioSource>().clip = finish;
        cam.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(6.0f);
        black.a = 0;
        backFade.GetComponent<SpriteRenderer>().color = black;
        cam.GetComponent<fading>().FadeOutTransition(2);
    }
    
    IEnumerator getInPipe() {
        throughPipe = true;
        model.GetComponent<BoxCollider>().enabled = false;
        player.GetComponent<AudioSource>().clip = pipe;
        player.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.7f);
        //throughPipe = false;
        GameObject.Find("gamestate MASTER").GetComponent<gameStateManager>().setCurrentLevel(2);
        cam.GetComponent<fading>().FadeOutTransition(2);
    }
    
    IEnumerator lostLife() {
    	paused = true;
    	lost = true;
		cam.GetComponent<AudioSource>().Pause();
    	player.GetComponent<AudioSource>().clip = gameOver;
        player.GetComponent<AudioSource>().Play();
    	yield return new WaitForSeconds(1);
    	model.GetComponent<CapsuleCollider>().enabled = false;
    	yield return new WaitForSeconds(2.5f);
    	GameObject.Find("gamestate MASTER").GetComponent<gameStateManager>().changeLifes(-1);
    	GameObject.Find("gamestate MASTER").GetComponent<gameStateManager>().setCurrentLevel(2);
        cam.GetComponent<fading>().FadeOutTransition(2);
    }

    public void triggerGameOver() {
    	StartCoroutine("lostLife");
    }

    void Update () {

        if (currentLvl > 2) {
            // prevent model from turning sideways:
            // deprecated, use instead "rigibody.constraints"
            //player.transform.rotation = Quaternion.Euler(lockPosX, transform.rotation.eulerAngles.y, lockPosZ);

            // update scores
            lifesObject.GetComponent<Text>().text = "x " + master.GetComponent<gameStateManager>().getLifes().ToString();
			coinsObject.GetComponent<Text>().text = "x " + master.GetComponent<gameStateManager>().getCoins().ToString();
			scoreObject.GetComponent<Text>().text = master.GetComponent<gameStateManager>().getScore().ToString();
			starsObject.GetComponent<Text>().text = "x " + master.GetComponent<gameStateManager>().getStars().ToString();
			bannerScoreObject.GetComponent<Text>().text = master.GetComponent<gameStateManager>().getBannerScore().ToString();
	
			// update time display
			if(running && !paused) {
				timeLeft = (int)(301 - Time.timeSinceLevelLoad);
				timeDisplayed.GetComponent<Text>().text = timeLeft.ToString();
			}
			// if time runs out -> gameover
			if(timeLeft <= 0) {
				GameObject.Find("gamestate MASTER").GetComponent<gameStateManager>().setCurrentLevel(2);
				Time.timeScale = 1;
				SceneManager.LoadScene("map");
			}

			// camera controller
			if(!lost) {
				if (cam.transform.position.x > camPosX1 + model.transform.position.x)
					cam.transform.Translate(Vector3.left * (camSlopeX * Mathf.Abs(cam.transform.position.x - model.transform.position.x)));
				if (cam.transform.position.x < camPosX2 + model.transform.position.x)
					cam.transform.Translate(Vector3.right * (camSlopeX * Mathf.Abs(cam.transform.position.x - model.transform.position.x)));
				if (cam.transform.position.y > camPosY1 + model.transform.position.y)
					cam.transform.Translate(Vector3.down * (camSlopeY * Mathf.Abs(cam.transform.position.y - model.transform.position.y)));
				if (cam.transform.position.y < camPosY2 + model.transform.position.y)
					cam.transform.Translate(Vector3.up * (camSlopeY * Mathf.Abs(cam.transform.position.y - model.transform.position.y)));
			}	
			if (running && !lost && !paused && !throughPipe) {
				// movement
				if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftShift))
					player.transform.Translate(Vector3.right * (0.1f + speed));
				if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.LeftShift))
					player.transform.Translate(Vector3.left * (0.1f + speed));
				if (Input.GetKey(KeyCode.RightArrow)) 
					player.transform.Translate(Vector3.right * 0.1f);
				if (Input.GetKey(KeyCode.LeftArrow)) 
					player.transform.Translate(Vector3.left * 0.1f);
				if (Input.GetKey(KeyCode.UpArrow)) 
					player.transform.Translate(Vector3.up * 0.1f);
				if (Input.GetKey(KeyCode.DownArrow)) 
					player.transform.Translate(Vector3.down * 0.1f);
				if (Input.GetKeyDown(KeyCode.UpArrow))
					player.GetComponent<AudioSource>().Play();
	            
				// controller style
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
                if (Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.L))
                    model.transform.Translate(Vector3.up * 0.159f);
				if (Input.GetKey(KeyCode.S))
                    model.transform.Translate(Vector3.down * 0.1f);
				if (Input.GetKeyDown(KeyCode.K)) {
					player.GetComponent<AudioSource>().clip = jump;
					player.GetComponent<AudioSource>().Play();
				}
				if (Input.GetKeyDown(KeyCode.L)) {
					player.GetComponent<AudioSource>().clip = spinjump;
					player.GetComponent<AudioSource>().Play();
				}
                if (Input.GetKey(KeyCode.L))
                    model.transform.Rotate(Vector3.up * Time.deltaTime * rotFact);
			}

			// pause and quit (space)
			if (Input.GetKeyDown(KeyCode.P) && !lost) {
     
                Debug.Log("model: " + model.transform.TransformPoint(model.transform.position));

                switch (paused) { 
				case true:
					sfx.GetComponent<AudioSource>().Play();
					cam.GetComponent<AudioSource>().UnPause();
					paused = false;
					Time.timeScale = 1;
					break;
				case false:
					sfx.GetComponent<AudioSource>().Play();
					cam.GetComponent<AudioSource>().Pause();
					paused = true;
					Time.timeScale = 0;
					break;
				}
			}

            if (Input.GetKey(KeyCode.Space) && paused && !lost)
            {
                sfx.GetComponent<AudioSource>().Play();
                Time.timeScale = 1;
                GameObject.Find("gamestate MASTER").GetComponent<gameStateManager>().setCurrentLevel(2);
                cam.GetComponent<fading>().FadeOutTransition(2);
                //SceneManager.LoadScene("map");
            }
            /*
			if (Input.GetKeyDown(KeyCode.P) && paused) {
				sfx.GetComponent<AudioSource>().Play();
				cam.GetComponent<AudioSource>().Play();
				paused = false;
				Time.timeScale = 1;
			}
			if (Input.GetKeyDown(KeyCode.P) && !paused) {
				sfx.GetComponent<AudioSource>().Play();
				cam.GetComponent<AudioSource>().Stop();
				paused = true;
				Time.timeScale = 0;
			}
			*/

            // shift (developer mode)
            /*
            if (Input.GetKey(KeyCode.CapsLock)) 
            		player.transform.Translate(Vector3.forward * 0.1f);
            if (Input.GetKey(KeyCode.Space)) 
            	player.transform.Translate(Vector3.back * 0.1f);
            */

            // camera controls
            /*
            if (Input.GetKey(KeyCode.Q)) {
				int i = 0;
				while(i < 90) {
					cam.transform.Rotate(Vector3.up * Time.deltaTime * 1.5f);
					cam.transform.Translate(Vector3.left * 0.002f);
					cam.transform.Translate(Vector3.forward * 0.001f);
					i++;
				}
			}
			if (Input.GetKey(KeyCode.E)) {
				int i = 0;
				while(i < 90) {
					cam.transform.Rotate(Vector3.down * Time.deltaTime * 1.5F);
					cam.transform.Translate(Vector3.right * 0.002f);
					cam.transform.Translate(Vector3.back * 0.001f);
					i++;
				}
			}
            */

            if (!lost && pipes_x.Length > 0 && model.transform.position.x > pipes_x[0] - 0.5 && model.transform.position.x < pipes_x[0] + 0.5 && (Input.GetKey(KeyCode.S))) {

                if (throughPipe == false)
                    StartCoroutine("getInPipe");

                if(throughPipe) {
                    model.transform.rotation = Quaternion.Euler(Vector3.up * 180);
                    model.transform.Translate(Vector3.down * Time.deltaTime * 0.5f);
                }
            }

			if (model.transform.position.x > levelEnd || Input.GetKeyDown(KeyCode.T)) {
				if(running) {
					running = false;
					GameObject.Find("gamestate MASTER").GetComponent<gameStateManager>().setCurrentLevel(2);
    	            StartCoroutine("finishSequence");
        	        //cam.GetComponent<fading>().FadeOutTransition(2);
            	    //SceneManager.LoadScene("map");
				}
			}

		}
	}
}
