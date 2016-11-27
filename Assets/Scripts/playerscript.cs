using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class playerscript : NetworkBehaviour {

	public float moveSpeed = 0.03f;
	private Rigidbody2D myBody;
	// Use this for initialization
	void Start () {
		myBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			checkMovement ();
		}
	}

	private void checkMovement() {
		transform.position = new Vector2 ((transform.position.x + Input.GetAxis ("Horizontal") * moveSpeed),transform.position.y + Input.GetAxis ("Vertical") * moveSpeed);;
	}
}
