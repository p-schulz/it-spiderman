using UnityEngine;
using System.Collections;

public class coin : MonoBehaviour {

    public GameObject mesh;
    public GameObject starFX;

	IEnumerator coinCollected() {

        mesh.GetComponent<MeshRenderer>().enabled = false;
        starFX.GetComponent<ParticleSystem>().Play();
		gameObject.GetComponent<AudioSource>().Play();
		GameObject.Find("GameState").GetComponent<GameState>().writeToConsole("coin collected");
        GameObject.Find("GameState").GetComponent<GameState>().addCoins(1);
        GameObject.Find("GameState").GetComponent<GameState>().addScore(100);
        yield return new WaitForSeconds(0.419f);
		gameObject.SetActive(false);
	}

	void OnTriggerEnter (Collider other) {
		Destroy(gameObject.GetComponent<BoxCollider>());
		StartCoroutine("coinCollected");
	}

	void Update() {
		//transform.Rotate(Vector3.up * Time.deltaTime * 25);
	}
}
