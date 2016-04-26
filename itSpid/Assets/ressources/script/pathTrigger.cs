using UnityEngine;
using System.Collections;

public class pathTrigger : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		Debug.Log("path triggered" + other.transform.gameObject);
	}
}