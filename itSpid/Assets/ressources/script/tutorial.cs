using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class tutorial : MonoBehaviour {

    public Text subtext;

    // tutorial messages
    public string pausing = "First of all: \n You can pause your game at any time by pressing 'Escape'.";
    public string movement01 = "Basic instincts: \n Use 'A' and 'D' to walk around.";
    public string movement02 = "Eat pasta - run fasta: \n Hold down 'J' to start running.";
    public string movement03 = "Did you already know? \n Press 'K' to jump!";
    public string fighting01 = "Teach those bad boys a lesson: \n Use 'I' to perform a punch.";
    public string fighting02 = "Kick-Ass: \n Press 'L' to smash your enemies with your foot!";
    public string fighting03 = "Watch out: Enemies will start tracking you \n down as you aproach to them...";
    public string hud01 = "Just for you information: \n Did you already see these fancy symbols in the corner?";
    public string hud02 = "The number underneath the Italian Spiderman Logo \n represents your remaining lifes!";
    public string hud03 = "The slider right of it shows your vitality. \n Eat some pasta or have a smoke to regenerate.";
    public string hud04 = "Once the slider reaches the left end \n the game is over and you have to start from the beginning.";

	// Use this for initialization
	void Start ()
    {
        StartCoroutine("messageCall");
	}
	
    IEnumerator messageCall()
    {
        yield return new WaitForSeconds(1);
        subtext.text = pausing;

        yield return new WaitForSeconds(9);
        subtext.text = movement01;

        yield return new WaitForSeconds(5);
        subtext.text = movement02;

        yield return new WaitForSeconds(5);
        subtext.text = movement03;

        yield return new WaitForSeconds(5);
        subtext.text = fighting01;

        yield return new WaitForSeconds(5);
        subtext.text = fighting02;

        yield return new WaitForSeconds(5);
        subtext.text = hud01;

        yield return new WaitForSeconds(5);
        subtext.text = hud02;

        yield return new WaitForSeconds(5);
        subtext.text = hud03;

        yield return new WaitForSeconds(5);
        subtext.text = hud04;

        yield return new WaitForSeconds(5);
        subtext.text = fighting03;

        yield return new WaitForSeconds(5);
        subtext.text = "";
    }

	// Update is called once per frame
	void Update () {
	}
}
