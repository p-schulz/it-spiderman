using UnityEngine;
using System.Collections;

public class behaviour_coopas : MonoBehaviour {

    public bool walk = true;
    public bool wings = false;
    public bool jump = false;
    public bool fly = false;
    public bool horizFly = false;
    public string color = "red";
    public GameObject player;

    bool direction; // 0=left, 1=right
    bool dashed;
    bool shell;
    bool hit;
    float pos;

    IEnumerator stomped() {
        dashed = true;
        yield return new WaitForSeconds(15);
        if (shell)
            shell = false;
    }

    public void stompOn() {
        GameObject.Find("GameState").GetComponent<GameState>().sfx2.clip = GameObject.Find("GameState").GetComponent<GameState>().kick;
        GameObject.Find("GameState").GetComponent<GameState>().sfx2.Play();
        if (wings || fly) {
            wings = false;
            fly = false;
        }
        if(shell)
            StartCoroutine("stomped");
        if(!shell) {
            GameObject.Find("GameState").GetComponent<GameState>().addScore(300);
            Destroy(gameObject);
        }
    }

    public void rotJump() {
        GameObject.Find("GameState").GetComponent<GameState>().addScore(300);
        Destroy(gameObject);
    }
	
    void OnCollisionStay(Collision coll) {
        if(coll.contacts.Length > 0) {
            ContactPoint contact = coll.contacts[0];
            Debug.DrawRay(contact.point, contact.normal, Color.red);

            if (contact.point.y > 0) {
                GameObject.Find("GameState").GetComponent<GameState>().writeToConsole("stompOn(coopa)");
                stompOn();
            }
            else if (!hit && coll.transform.gameObject.name == "Mario") {
                hit = true;
                player.GetComponent<CharController>().triggerGameOver();
            }
            else if (!hit && coll.transform.gameObject.name == "wallEdge" && direction) {
                direction = false;
            }
            else if (!hit && coll.transform.gameObject.name == "wallEdge" && !direction) {
                direction = true;
            }
        }
    }

    /*
    void OnTriggerEnter(Collider other) {
        if(!hit && other.transform.gameObject.name == "Mario") {
            hit = true;
            player.GetComponent<CharController>().triggerGameOver();
        }
        if(!hit && other.transform.gameObject.name == "wallEdge") {
            GameObject.Find("GameState").GetComponent<GameState>().writeToConsole("coopa is turning");
            direction = !direction;
            if(!direction)
                gameObject.transform.Rotate(Vector3.forward * 180);
            if(direction)
                gameObject.transform.Rotate(Vector3.forward * -180);  
        }
    }
    */

    void Start() {
        direction = false;
        dashed = false;
        shell = false;
        hit = false;
        player = GameObject.Find("player");
        pos = gameObject.transform.position.x;
	}

    void Update() {
        if(walk && !hit) {
            if (!direction)
                gameObject.transform.Translate(Vector3.left * Time.deltaTime * 1.5f);
            if (direction)
                gameObject.transform.Translate(Vector3.right * Time.deltaTime * 1.5f);
            /*
			if(gameObject.transform.position.x >= pos + 3) {
                direction = false;
				gameObject.transform.Rotate(Vector3.forward * 180);
			}
			if(gameObject.transform.position.x <= pos - 3) {
                direction = true;
				gameObject.transform.Rotate(Vector3.forward * -180);
			}
            */
        }
        if(wings && !hit) {
            if(jump)
                Debug.Log("jumping with wings");

            if(fly) {
                if(horizFly)
                    Debug.Log("flying horizontal");
                if(!horizFly)
                    Debug.Log("flying vertically");
            }
        }
	}
}
