using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class map : MonoBehaviour {
	
	public AudioSource clipMap;
	public GameObject fadeBlack;
	public GameObject mStarts;
	public GameObject maincam;
	bool loadLevel = false;
	bool fadein = true;
	public float fadeinSpeed = 0.03f;
	public float fadeoutSpeed = 0.1f;
	int levelIndex = 3;
	Color alphaFade;

	public bool overview = true;
	Vector3 lastPos;

	// public methods
	public void setLevelIndex(int i) {
		levelIndex = i;
	}
	public int currentLevel() {
		return levelIndex;
	}

	void Start () {
		alphaFade.a = 1.0f;
		alphaFade.r = 0;
		alphaFade.g = 0;
		alphaFade.b = 0;
		fadeBlack.GetComponent<Image>().color = alphaFade;
		mStarts.SetActive(false);
		//maincam = GameObject.Find("Main Camera");
		GameObject.Find("gamestate MASTER").GetComponent<gameStateManager>().setCurrentLevel(2);
	}

	void Update () {

		// (de-)activate map exploration
		/*
		if(Input.GetKeyDown(KeyCode.P) && !overview) {
			lastPos = maincam.transform.position;
			overview = true;
			//Debug.Log("exploration active" + overview);
		}
		if(Input.GetKeyDown(KeyCode.P) && overview) {
			maincam.transform.position = lastPos;
			overview = false;
			//Debug.Log("exploration disabled" + overview);
		}
		*/

		// movement in exploration mode
		if(overview) {
			
			if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) 
				maincam.transform.Translate(Vector3.right * 0.1f);
			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) 
				maincam.transform.Translate(Vector3.left * 0.1f);
			
			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
				maincam.transform.Translate(Vector3.forward * 0.1f);
				//maincam.transform.Translate(Vector3.up * 0.033f);
			}
			if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) { 
				maincam.transform.Translate(Vector3.back * 0.1f);
				//maincam.transform.Translate(Vector3.down * 0.033f);
			}
		}	

		// fade in from black
		if(fadein) {
			alphaFade.a -= fadeinSpeed;
			fadeBlack.GetComponent<Image>().color = alphaFade;
		}
		if(fadein && alphaFade.a >= 1)
			fadein = false;

		// music fade out and level loading
		if(Input.GetKey(KeyCode.Return)) {
			loadLevel = true;
			GameObject.Find("gamestate MASTER").GetComponent<gameStateManager>().setNextLevel(levelIndex);
		}
		if(loadLevel && clipMap.volume > 0) {
			clipMap.volume -= 0.005f;
			alphaFade.a += fadeoutSpeed;
			fadeBlack.GetComponent<Image>().color = alphaFade;
		}
		if(loadLevel && clipMap.volume <= 0) {
			mStarts.SetActive(true);
			GameObject.Find("gamestate MASTER").GetComponent<gameStateManager>().setCurrentLevel(levelIndex);
			SceneManager.LoadScene(levelIndex);
		}

	}
}
