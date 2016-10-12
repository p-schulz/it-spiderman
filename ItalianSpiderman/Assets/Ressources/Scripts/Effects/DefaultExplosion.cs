using UnityEngine;
using System.Collections;

public class DefaultExplosion : MonoBehaviour {

    public float loopduration;
    
	// Update is called once per frame
	void Update () {
        float r = Mathf.Sin((Time.time / loopduration) * (2 * Mathf.PI)) * 0.5f + 0.25f;
        float g = Mathf.Sin((Time.time / loopduration + 0.33333333f) * 2 * Mathf.PI) * 0.5f + 0.25f;
        float b = Mathf.Sin((Time.time / loopduration + 0.66666667f) * 2 * Mathf.PI) * 0.5f + 0.25f;
        float correction = 1 / (r + g + b);
        r *= correction;
        g *= correction;
        b *= correction;
        gameObject.GetComponent<Renderer>().material.SetVector("_ChannelFactor", new Vector4(r, g, b, 0));
    }
}
