using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float movespeed;
    public cameraScript myCameraScript;

    private float playerInputVertical;
    private float playerInputHorizontal;
    private float playerInputQundE;
    private float playerInputJump;
    private RaycastHit clicked;
    private Ray myRay;

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
        playerInputJump= Input.GetAxis("Jump");

        if (Input.GetMouseButton(0))
        {
            myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(myRay, out clicked))
            {
                //TODO: Hier noch pruefen welches Objekt angeklickt wurde
                if (clicked.transform.GetComponent<Rigidbody>() != null)
                {
                    clicked.transform.GetComponent<Rigidbody>().AddForce(Vector3.up * 70);
                }
                
            }
        }

        if (playerInputJump > 0)
        {
            myBody.AddForce(Vector3.up * 100);
        }

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
