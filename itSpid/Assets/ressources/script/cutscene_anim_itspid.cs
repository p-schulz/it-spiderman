using UnityEngine;
using System.Collections;

public class cutscene_anim_itspid : MonoBehaviour {

	public Animation anim;
	public AnimationClip animation01;


	public bool anim01;


	void Update () {
	
		if(anim01) {
			anim01 = false;
			anim = gameObject.GetComponent<Animation>();
			anim.clip = animation01;
			anim.Play();
		}
	}
}
