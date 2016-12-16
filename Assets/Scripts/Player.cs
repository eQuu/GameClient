using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float movespeed;
    public cameraScript myCameraScript;

    private float playerInputVertical;
    private float playerInputHorizontal;
    private float playerInputQundE;

    private Rigidbody myBody;

    // Use this for initialization
    void Start () {
        myBody = GetComponent<Rigidbody>();
	}

    void Update()
    {
        playerInputVertical = Input.GetAxis("Vertical");
        playerInputHorizontal = Input.GetAxis("Horizontal");
        playerInputQundE = Input.GetAxis("QundE");

        if (playerInputQundE < 0)
        {
            myBody.transform.position = myBody.transform.position + transform.right * movespeed * Time.deltaTime;
        } else if (playerInputQundE > 0)
        {
            myBody.transform.position = myBody.transform.position - transform.right * movespeed * Time.deltaTime;
        }

        if (playerInputHorizontal != 0)
        {
            transform.Rotate(Vector3.up, playerInputHorizontal);
            myCameraScript.followRotation(playerInputHorizontal);

        }
        if (playerInputVertical > 0)
        {
            myBody.transform.position = myBody.transform.position + transform.forward * movespeed * Time.deltaTime;
        } else if (playerInputVertical < 0)
        {
            myBody.transform.position = myBody.transform.position - transform.forward * movespeed * Time.deltaTime;
        }
    }
}
