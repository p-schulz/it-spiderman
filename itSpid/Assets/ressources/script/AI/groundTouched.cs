using UnityEngine;
using System.Collections;

public class groundTouched : MonoBehaviour {

	public GameObject player;
	bool hit = false;

	void OnTriggerEnter(Collider other) {
        if(!hit && other.transform.gameObject.name == "Mario") {
            hit = true;
            player.GetComponent<CharController>().triggerGameOver();
        }
    }

    void Start() {
		player = GameObject.Find("player");
    }
}
