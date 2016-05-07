using UnityEngine;
using System.Collections;

public class trigger_cutscene1 : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (collision.transform.name == "Model")
                GameObject.Find("GameState").GetComponent<fading>().FadeOutTransition(2);
        }
    }

}