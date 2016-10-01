using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelExit : MonoBehaviour {
    public AudioSource defaultMusic;
    public GameObject finishLine;

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            StartCoroutine("Leave");
        }
    }

    IEnumerator Leave()
    {
        defaultMusic.Stop();
        finishLine.SetActive(true);
        gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(5);
        GameObject.FindObjectOfType<GameMaster>().GameOver();
    }

    // Update is called once per frame
    void Update () {
	
	}
}
