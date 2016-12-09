using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float movespeed;
    private Rigidbody myBody;

	// Use this for initialization
	void Start () {
        myBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        myBody.AddForce(movement * movespeed);
    }
}
