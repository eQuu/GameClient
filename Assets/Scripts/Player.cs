using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float movespeed;

    private Rigidbody myBody;

    // Use this for initialization
    void Start () {
        myBody = GetComponent<Rigidbody>();
	}

    void Update()
    {
        if (Input.GetAxis("Horizontal")!= 0)
        {
            transform.Rotate(Vector3.up, Input.GetAxis("Horizontal"));
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            myBody.transform.position = myBody.transform.position + transform.forward * movespeed * Time.deltaTime;
        } else if (Input.GetAxis("Vertical") < 0)
        {
            myBody.transform.position = myBody.transform.position - transform.forward * movespeed * Time.deltaTime;
        }



    }
}
