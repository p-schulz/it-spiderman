using UnityEngine;
using System.Collections;

public class Cigarette : MonoBehaviour {

    public int Value = 1;
    public bool UniformHeight = true;
    public float PlacementHeight = 1f;
    public float SpinSpeed = 150.0f;

    public GameObject DeathEffect;
    public Transform Art;

    public AudioClip CoinSound;

    // Use this for initialization
    void Start () {
        RaycastHit hit;

        if (UniformHeight && Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity)) {
            transform.position = hit.point + Vector3.up * PlacementHeight;
        }
    }
	
	// Update is called once per frame
	void Update () {
        Art.rotation *= Quaternion.AngleAxis(SpinSpeed * Time.deltaTime, transform.up);
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player") {
            Death();
        }
    }

    void Death() {
        // add life
        GameObject.FindObjectOfType<PlayerStatus>().AddHealth(Value);
        // play sound
        GameObject.FindObjectOfType<PlayerSound>().PlaySmokeCigarette();

        if (DeathEffect)
            Instantiate(DeathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
