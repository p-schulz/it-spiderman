using UnityEngine;
using System.Collections;

public class basic_behaviour : MonoBehaviour {

	public GameObject model;
    public CharController chara;
    public Animation anim;
    public CapsuleCollider coll1;
    public CapsuleCollider coll2;
    public SphereCollider head;
    public CapsuleCollider[] parts1;
    public BoxCollider[] parts2;
    public float triggerDistance = 8.0f;
    public float triggerClosure = 2.0f;
	public float speed = 3.5f;
    public bool dull = false;
    public bool defeated = false;
    
    CharController ctrl;
    bool walking = false;
    bool direction = false; // 1 = left, 0 = right
    bool turned = false;
    bool won = false;
    bool close = false;
    bool paused = false;
	
    public bool getState()
    {
        return defeated;
    }

    void Start()
    {
        ctrl = GameObject.Find("player").GetComponent<CharController>();
        if (!dull)
        {
            coll1.enabled = true;
            coll2.enabled = true;
            head.enabled = false;
            foreach (CapsuleCollider capsule in parts1)
                capsule.enabled = false;
            foreach (BoxCollider boxx in parts2)
                boxx.enabled = false;
        }
    }

	void OnTriggerEnter(Collider other) {
        if(other.transform.gameObject.name == "Model") {
            GameObject.Find("GameState").GetComponent<GameState>().health -= 10;
            Debug.Log("got hit by enemy");
            //charctrl.GetComponent<ctrl>().triggerGameOver();
        }
    }
    IEnumerator hitting()
    {
        yield return new WaitForSeconds(0.5f);
        won = true;
    }
    IEnumerator gotHit()
    {
        defeated = true;
        if (!dull)
        {
            yield return new WaitForSeconds(0.2f);
            head.enabled = true;
            foreach (CapsuleCollider capsule in parts1)
                capsule.enabled = true;
            foreach (BoxCollider boxx in parts2)
                boxx.enabled = true;
            yield return new WaitForSeconds(0.2f);
            coll1.enabled = false;
            coll2.enabled = false;
        }
        if (dull)
        {
            yield return new WaitForSeconds(0.5f);
            coll1.enabled = false;
            yield return new WaitForSeconds(0.65f);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!defeated)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (collision.transform.name == "Model")
                {
                    if (chara.hitting || chara.punching)
                    {
                        defeated = true;
                        GameObject.Find("GameState").GetComponent<GameState>().enemies++;
                        close = true;
                        anim["knocked_down"].speed = 4;
                        anim.Play("knocked_down");
                        gameObject.GetComponent<Rigidbody>().AddForce(contact.normal * -100);
                        StartCoroutine("gotHit");
                    }
                    if (!chara.hitting && !chara.punching)
                    {
                        GameObject.Find("player").GetComponent<CharController>().StartCoroutine("gotHit");
                        anim["right_hook"].speed = 2;
                        anim.Play("right_hook");
                        close = true;
                        StartCoroutine("hitting");
                        model.GetComponent<Rigidbody>().AddForce(contact.normal * -100);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        paused = ctrl.getPause();
        if (!defeated) {
            turned = false;
            if (!walking && (gameObject.transform.position.x <= model.transform.position.x + triggerDistance
                && gameObject.transform.position.x > model.transform.position.x - triggerDistance)) {
                walking = true;
                Debug.Log("started walking");
            }
            if (walking && (gameObject.transform.position.x >= model.transform.position.x + triggerDistance
                || gameObject.transform.position.x < model.transform.position.x - triggerDistance)) {
                walking = false;
                Debug.Log("stopped walking");
            }
            /*
            if (walking && (gameObject.transform.position.x > model.transform.position.x - triggerClosure && !direction) || (gameObject.transform.position.x < model.transform.position.x + triggerClosure && direction)) {
                close = true;
                Debug.Log("too close");
            }
            if (!walking && (gameObject.transform.position.x < model.transform.position.x - triggerClosure && !direction) || (gameObject.transform.position.x > model.transform.position.x + triggerClosure && direction)) {
                close = false;
                Debug.Log("not too close");
            }
            */
            if (!walking && !close || won) {
                anim.Play("idle");
            }
            if (walking && !close && !won) {
                turned = false;
                anim.Play("running_inPlace");
                if (!direction && gameObject.transform.position.x > model.transform.position.x) {
                    gameObject.transform.rotation = Quaternion.Euler(Vector3.up * 270);
                    direction = true; turned = true;
                    Debug.Log("right");
                }
                if (!turned && direction && gameObject.transform.position.x < model.transform.position.x) {
                    gameObject.transform.rotation = Quaternion.Euler(Vector3.up * 90);
                    direction = false;
                    Debug.Log("left");
                }
                if (!paused)
                    gameObject.transform.Translate(Vector3.forward * Time.deltaTime * speed);

            }
        }
    }
}
