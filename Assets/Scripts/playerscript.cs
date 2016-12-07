using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class playerscript : NetworkBehaviour {

    public float moveSpeed = 0.03f;

	private Rigidbody2D myBody;
    private GameObject myTarget;
    private gamescript myGameScript;


    // Use this for initialization
    void Start() {
        myBody = GetComponent<Rigidbody2D>();
        GameObject myGame = GameObject.Find("Game");
        myGameScript = myGame.GetComponent<gamescript>();
        myGameScript.setLocalPlayer(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			checkMovement ();
		}
	}

    void OnMouseDown()
    {
        myGameScript.newTarget(gameObject);
    }

	private void checkMovement() {
		transform.position = new Vector2 ((transform.position.x + Input.GetAxis ("Horizontal") * moveSpeed),transform.position.y + Input.GetAxis ("Vertical") * moveSpeed);;
	}
}
