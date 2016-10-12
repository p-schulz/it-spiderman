using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
    public GameObject WhiteMatte;
    public GameObject BlackMatte;
    bool fading = false;
	
    public void StartNewGame()
    {
        WhiteMatte.SetActive(true);
        fading = true;
        StartCoroutine("LoadLevel01");
    }

    public void ExitButton()
    {
        BlackMatte.SetActive(true);
        fading = true;
        StartCoroutine("QuitGame");
    }


    IEnumerator LoadLevel01()
    {
        yield return new WaitForSeconds(5);
        Application.LoadLevel(1);
    }

    IEnumerator QuitGame()
    {
        yield return new WaitForSeconds(5);
        Application.Quit();
    }

    // Update is called once per frame
    void Update () {
        if (fading)
            gameObject.GetComponent<AudioSource>().volume -= 0.0025f;
	}
}
