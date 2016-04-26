using UnityEngine;
using System.Collections;

public class finishLine : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.transform.name == "Mario")
			GameObject.Find("player").GetComponent<CharController>().finish = true;
	}
}
