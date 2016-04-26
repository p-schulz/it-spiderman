using UnityEngine;
using System.Collections;

public class behaviour_rocketBomb : MonoBehaviour {

	public GameObject mario;
	public GameObject charctrl;
	public float triggerDistance = 9.5f;
	public float speed = 3.5f;

	bool hit = false;
	bool flying = false;

	IEnumerator shoot() {
		gameObject.GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(0.5f);
	}

	public void goOff() {
		flying = true;
		gameObject.GetComponent<AudioSource>().Play();
	}
	
	void OnTriggerEnter(Collider other) {
        if(!hit && other.transform.gameObject.name == "Mario") {
            hit = true;
            charctrl.GetComponent<ctrl>().triggerGameOver();
        }
    }

	// Update is called once per frame
	void Update () {
		if(!flying && gameObject.transform.position.x <= mario.transform.position.x + triggerDistance)
			goOff();

		if(!hit && flying)
			gameObject.transform.Translate(Vector3.right * Time.deltaTime * speed);
	}
}
