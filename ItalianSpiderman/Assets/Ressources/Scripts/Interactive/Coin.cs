using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

    public int Value = 1;
    public bool UniformHeight = true;
    public float PlacementHeight = 0.6f;
    public float SpinSpeed = 360.0f;
    public GameObject DeathEffect;
    public AudioClip CoinSound;
    public Transform Art;

    void Awake()
    {
        RaycastHit hit;

        if (UniformHeight && Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity))
        {
            transform.position = hit.point + Vector3.up * PlacementHeight;
        }
    }

    void Update()
    {
        Art.rotation *= Quaternion.AngleAxis(SpinSpeed * Time.deltaTime, transform.up);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Death();
        }
    }

    void Death()
    {
        GameObject.FindObjectOfType<GameMaster>().AddCoin(Value);
        GameObject.FindObjectOfType<PlayerStatus>().AddHealth(Value);

        if (DeathEffect)
            Instantiate(DeathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
