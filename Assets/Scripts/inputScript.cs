using UnityEngine;
using System.Collections;

public class inputScript : MonoBehaviour {

    public float movespeed;
    public cameraScript myCameraScript;
    public networkScript myNetwork;
    public uiScript myUiScript;
    public gameScript myGame;

    private float playerInputVertical;
    private float playerInputHorizontal;
    private float playerInputQundE;
    private float playerInputJump;
    private RaycastHit clicked;
    private Ray myRay;
    private Animator myAnim;
    private bool hasMoved = false;
    private float updateRate = 0.1f;
    private float lastUpdateTime = 0f;

    private Rigidbody myBody;

    // Use this for initialization
    void Start () {
        myBody = GetComponent<Rigidbody>();
        myAnim = GetComponent<Animator>();
	}

    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 0.1f);
    }

    void Update()
    {
        playerInputVertical = Input.GetAxis("Vertical");
        playerInputHorizontal = Input.GetAxis("Horizontal");
        playerInputQundE = Input.GetAxis("QundE");
        playerInputJump= Input.GetAxis("Jump");
        hasMoved = false;

        if (Input.GetMouseButtonUp(0))
        {
            myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(myRay, out clicked))
            {
                if (clicked.transform.tag.Contains("trgt"))
                {
                    myUiScript.setTarget(clicked.transform);
                } else
                {
                    myUiScript.setTarget(null);
                }
            } else
            {
                myUiScript.setTarget(null);
            }
        }

        if (Input.GetKeyDown("space") && isGrounded())
        {
            myBody.AddForce(Vector3.up * 300);
            hasMoved = true;
        }

        if (playerInputQundE < 0)
        {
            myBody.transform.position = myBody.transform.position + transform.right * movespeed * Time.deltaTime;
            hasMoved = true;
        } else if (playerInputQundE > 0)
        {
            myBody.transform.position = myBody.transform.position - transform.right * movespeed * Time.deltaTime;
            hasMoved = true;
        }

        if (playerInputHorizontal != 0)
        {
            transform.Rotate(Vector3.up, playerInputHorizontal);
            myCameraScript.followRotation(playerInputHorizontal);
            //TODO: Rotation an den Server geben
        }
        if (playerInputVertical > 0)
        {
            myBody.transform.position = myBody.transform.position + transform.forward * movespeed * Time.deltaTime;
            myAnim.SetBool("isWalking", true);
            hasMoved = true;
        } else if (playerInputVertical < 0)
        {
            myBody.transform.position = myBody.transform.position - transform.forward * movespeed * Time.deltaTime;
            myAnim.SetBool("isWalking", true);
            hasMoved = true;
        } else
        {
            myAnim.SetBool("isWalking", false);
        }

        lastUpdateTime = lastUpdateTime + Time.deltaTime;
        if (hasMoved && lastUpdateTime > updateRate)
        {
            myNetwork.sendMessage("3;" + myGame.getMyPlayerId() + ";" + myGame.getMyPosInList() + ";" + myBody.transform.position.x + ";" + myBody.transform.position.y + ";" + myBody.transform.position.z);
            lastUpdateTime = 0f;
        }
    }
}
