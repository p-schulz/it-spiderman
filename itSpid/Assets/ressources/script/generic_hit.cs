using UnityEngine;
using System.Collections;

public class generic_hit : MonoBehaviour {
	public GameState gs;
	public CharController cc;

	int breakdown = 2;

	void Start() {
		gs = GameObject.Find("GameState").GetComponent<GameState>();
		cc = GameObject.Find("player").GetComponent<CharController>();
	}

	void OnCollisionEnter(Collision collision) {
		foreach (ContactPoint contact in collision.contacts) {
			if (collision.transform.name == "Model" && cc.punching && breakdown > 0) { 
				Debug.Log("table hit");
				gameObject.GetComponent<Rigidbody>().AddForce(contact.normal *  200); 
			}
		}
	}
}
