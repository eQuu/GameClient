using UnityEngine;
using System.Collections;

public class inputScript : MonoBehaviour {

    enum Mousestate : byte
    {
        Normal = 0,
        Aoespell = 1,
        Itemuse = 2,
        Spelluse = 3,
    };

    public cameraScript myCameraScript;
    public networkScript myNetwork;
    public uiScript myUiScript;
    public gameScript myGame;

    private playerScript myPlayer;
    private GameObject castIndicator;
    private Mousestate mouseState = 0;
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

    public void initiate(gameScript game, networkScript network, cameraScript camera, uiScript ui, Rigidbody body, Animator anim)
    {
        this.myGame = game;
        this.myNetwork = network;
        this.myCameraScript = camera;
        this.myUiScript = ui;
        this.myBody = body;
        this.myAnim = anim;
        this.castIndicator = myUiScript.castIndicator;
        this.castIndicator = Instantiate(castIndicator, Camera.main.transform.position, Camera.main.transform.rotation);
        this.castIndicator.SetActive(false);
        this.myPlayer = myBody.gameObject.GetComponent<playerScript>();
    }

    public void setBody(Rigidbody newBody)
    {
        this.myBody = newBody;
    }

    public void setAnimator(Animator newAnimator)
    {
        this.myAnim = newAnimator;
    }

    // Use this for initialization
    void Start () {
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

        //Auf den mouseState reagieren
        switch (mouseState)
        {
            case Mousestate.Normal:
                //Der Spieler waehlt ein Target
                if (Input.GetMouseButtonUp(0))
                {
                    //Input vom Chat wegnehmen
                    myUiScript.checkFocus();
                    myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(myRay, out clicked))
                    {
                        if (clicked.transform.tag.Contains("trgt"))
                        {
                            myUiScript.setTarget(clicked.transform.gameObject.GetComponent<playerScript>());
                        }
                        else
                        {
                            myUiScript.setTarget(null);
                        }
                    }
                    else
                    {
                        myUiScript.setTarget(null);
                    }
                }
                break;
            case Mousestate.Aoespell:
                //Wo soll der Aoespell hin
                myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(myRay, out clicked))
                {
                    Vector3 upwards = new Vector3(0, 1f, 0);
                    castIndicator.transform.position = clicked.point + upwards;
                    castIndicator.transform.rotation = Quaternion.AngleAxis(90.0f,Vector3.right);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    castIndicator.SetActive(false);
                    mouseState = Mousestate.Normal;
                    //TODO: SpellId einbauen
                    myNetwork.sendMessage("5;" + myGame.getMyPlayerId() + ";" + myGame.getMyPosInList() + ";" + clicked.point.x + ";" + clicked.point.y + ";" + clicked.point.z);
                }
                break;
            case Mousestate.Itemuse:
                //Wo soll das Item benutzt werden
                break;
            case Mousestate.Spelluse:
                //Wo soll der Spell benutzt werden
                break;
            default:
                break;
        }


        //Keyboard
        if (myUiScript.chatIsFocused())
        {
            myAnim.SetBool("isWalking", false);
            if (Input.GetKeyDown("return"))
            {
                myUiScript.sanitizeInput();
                myNetwork.sendMessage("2;");
                myUiScript.focusChat(false);
            }
        }
        else
        {
            if (Input.GetKeyDown("return"))
            {
                myUiScript.focusChat(true);
            }

            if (Input.GetKeyDown("1") && mouseState != Mousestate.Aoespell)
            {
                mouseState = Mousestate.Aoespell;
                castIndicator.SetActive(true);
            }

            if (Input.GetKeyDown("space") && isGrounded())
            {
                myBody.AddForce(Vector3.up * 300);
                hasMoved = true;
            }

            if (playerInputQundE < 0)
            {
                myBody.transform.position = myBody.transform.position + transform.right * this.myPlayer.getMovespeed() * Time.deltaTime;
                hasMoved = true;
            }
            else if (playerInputQundE > 0)
            {
                myBody.transform.position = myBody.transform.position - transform.right * this.myPlayer.getMovespeed() * Time.deltaTime;
                hasMoved = true;
            }

            if (playerInputHorizontal != 0)
            {
                transform.Rotate(Vector3.up, playerInputHorizontal);
                myCameraScript.followRotation(playerInputHorizontal);
                hasMoved = true;
            }
            if (playerInputVertical > 0)
            {
                myBody.transform.position = myBody.transform.position + transform.forward * this.myPlayer.getMovespeed() * Time.deltaTime;
                myAnim.SetBool("isWalking", true);
                hasMoved = true;
            }
            else if (playerInputVertical < 0)
            {
                myBody.transform.position = myBody.transform.position - transform.forward * this.myPlayer.getMovespeed() * Time.deltaTime;
                myAnim.SetBool("isWalking", true);
                hasMoved = true;
            }
            else
            {
                myAnim.SetBool("isWalking", false);
            }
        }
        lastUpdateTime = lastUpdateTime + Time.deltaTime;
        if (hasMoved && lastUpdateTime > updateRate)
        {
            myNetwork.sendMessage("3;" + myGame.getMyPlayerId() + ";" + myGame.getMyPosInList() + ";" + myBody.transform.position.x + ";" + myBody.transform.position.y + ";" + myBody.transform.position.z + ";" + myBody.transform.rotation.w + ";" + myBody.transform.rotation.x + ";" + myBody.transform.rotation.y + ";" + myBody.transform.rotation.z);
            lastUpdateTime = 0f;
        }
    }
}
