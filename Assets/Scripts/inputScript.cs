using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class inputScript : MonoBehaviour {

    enum Mousestate : byte
    {
        Normal = 0,
        Pointspell = 1,
        Itemuse = 2,
        TargetSpell = 3,
    };

    public cameraScript myCameraScript;
    public networkScript myNetwork;
    public uiScript myUiScript;
    public gameScript myGame;

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
    private bool isMouseMoving = false;
    private bool camWasMoved = false;
    private float updateRate = 0.1f;
    private float lastUpdateTime = 0f;
    private Stopwatch mouseTimer;
    private uint nextSpellId;

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
        mouseTimer = new Stopwatch();
	}

    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 0.1f);
    }

    private bool mouseRotationCheck()
    {
        if (mouseTimer.ElapsedMilliseconds < 1000 && camWasMoved == false)
        {
            return false;
        } else
        {
            return true;
        }
    }

    private void setOwnTarget(playerScript newTarget)
    {
        UnityEngine.Debug.Log("myposinlist: " + myGame.getMyPosInList() + ", myownplayerposinlist: " + myGame.getOwnPlayer().getListPos());
        if (myUiScript.getTarget() != newTarget)
        {
            UnityEngine.Debug.Log("neues Target");
            myGame.setTarget(myGame.getMyPosInList(), newTarget);
            if (newTarget == null)
            {
                myNetwork.sendMessage("16;" + myGame.getMyPlayerId() + ";" + myGame.getMyPosInList() + ";" + "null");
            }
            else
            {
                myNetwork.sendMessage("16;" + myGame.getMyPlayerId() + ";" + myGame.getMyPosInList() + ";" + newTarget.getListPos());
            }
        }
    }

    void Update()
    {
        playerInputVertical = Input.GetAxis("Vertical");
        playerInputHorizontal = Input.GetAxis("Horizontal");
        playerInputQundE = Input.GetAxis("QundE");
        playerInputJump= Input.GetAxis("Jump");
        hasMoved = false;

        //Input evtl. vom Chat wegnehmen
        myUiScript.checkFocus();

        //Mouse
        //Wenn gescrollt wird...
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            //...checken ob die Maus ueberm Chat liegt...
            if (myUiScript.getMouseOverChat())
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    myUiScript.scrollChat(-1);
                }
                else
                {
                    myUiScript.scrollChat(1);
                }
            }
            //...ansonsten zoomen
            else
            {
                myCameraScript.zoomCamera();
            }
        }

        //Maustimer anwerfen, falls die linke Maustaste gehalten wird
        if (Input.GetMouseButtonDown(0))
        {
            mouseTimer.Reset();
            mouseTimer.Start();
            camWasMoved = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseTimer.Stop();
            isMouseMoving = false;
        }

        if (Input.GetMouseButton(0))
        {
            if ((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) && camWasMoved == false && mouseTimer.ElapsedMilliseconds < 20)
            {
                camWasMoved = true;
            }
            myCameraScript.rotateCamera();
        }

        //Rechte Maustaste gehalten
        if (Input.GetMouseButton(1))
        {
            transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            myCameraScript.followRotation(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            //Und linke Maustaste gehalten
            if (Input.GetMouseButton(0)) {
                myBody.transform.position = myBody.transform.position + transform.forward * myGame.getOwnPlayer().getMovespeed() * Time.deltaTime;
                myAnim.SetBool("isWalking", true);
                hasMoved = true;
                isMouseMoving = true;
            }
        }

        //Auf den mouseState reagieren
        switch (mouseState)
        {
            case Mousestate.Normal:
                //Der Spieler waehlt ein Target
                if (Input.GetMouseButtonUp(0) && mouseRotationCheck() == false)
                {
                    myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(myRay, out clicked))
                    {
                        if (clicked.transform.tag.Contains("trgt"))
                        {
                            setOwnTarget(clicked.transform.gameObject.GetComponent<playerScript>());
                        }
                        else
                        {
                            setOwnTarget(null);
                        }
                    }
                    else
                    {
                        setOwnTarget(null);
                    }
                }
                break;
            case Mousestate.Pointspell:
                //Wo soll der Aoespell hin
                myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(myRay, out clicked))
                {
                    Vector3 upwards = new Vector3(0, 1f, 0);
                    castIndicator.transform.position = clicked.point + upwards;
                    castIndicator.transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.right);
                }
                if (Input.GetMouseButtonUp(0) && mouseRotationCheck() == false)
                {
                    castIndicator.SetActive(false);
                    mouseState = Mousestate.Normal;
                    myGame.tryPointSpellCast(nextSpellId, clicked.point);
                }
                break;
            case Mousestate.Itemuse:
                //Wo soll das Item benutzt werden
                break;
            case Mousestate.TargetSpell:
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
                string chatMsg = myUiScript.sanitizeInput();
                if (chatMsg != "")
                {
                    myNetwork.sendMessage("2;" + myGame.getMyPlayerId() + ";" + myGame.getMyPosInList() + ";" + chatMsg);
                }
                myUiScript.focusChat(false);
            }
            if (Input.GetKeyDown("escape"))
            {
                myUiScript.focusChat(false);
            }
        }
        else
        {
            if (Input.GetKeyDown("return"))
            {
                myUiScript.focusChat(true);
            }

            if (Input.GetKeyDown("space") && isGrounded())
            {
                myBody.AddForce(Vector3.up * 300);
                hasMoved = true;
            }

            if (playerInputQundE < 0)
            {
                myBody.transform.position = myBody.transform.position + transform.right * myGame.getOwnPlayer().getMovespeed() * Time.deltaTime;
                hasMoved = true;
            }
            else if (playerInputQundE > 0)
            {
                myBody.transform.position = myBody.transform.position - transform.right * myGame.getOwnPlayer().getMovespeed() * Time.deltaTime;
                hasMoved = true;
            }

            if (!isMouseMoving)
            {
                if (playerInputHorizontal != 0)
                {
                    transform.Rotate(Vector3.up, playerInputHorizontal);
                    myCameraScript.followRotation(playerInputHorizontal, 0);
                    hasMoved = true;
                }
                if (playerInputVertical > 0)
                {
                    myBody.transform.position = myBody.transform.position + transform.forward * myGame.getOwnPlayer().getMovespeed() * Time.deltaTime;
                    myAnim.SetBool("isWalking", true);
                    hasMoved = true;
                }
                else if (playerInputVertical < 0)
                {
                    myBody.transform.position = myBody.transform.position - transform.forward * myGame.getOwnPlayer().getMovespeed() * Time.deltaTime;
                    myAnim.SetBool("isWalking", true);
                    hasMoved = true;
                }
                else if (isMouseMoving == false)
                {
                    myAnim.SetBool("isWalking", false);
                }
            }
            if (Input.GetKeyDown("f"))
            {
                if (myUiScript.getTarget() != null)
                {
                    playerScript newTarget = myUiScript.getTarget().getTarget();
                    if ( newTarget != null)
                    {
                        myGame.setTarget(newTarget);
                    }
                }
            }

            //Spellbar
            if (Input.GetKeyDown("1") && mouseState != Mousestate.Pointspell)
            {
                nextSpellId = 0;
                mouseState = Mousestate.Pointspell;
                castIndicator.SetActive(true);
            }
            if (Input.GetKeyDown("2") && mouseState != Mousestate.Pointspell)
            {
                nextSpellId = 1;
                myGame.tryTargetSpellCast(nextSpellId);
            }
            if (Input.GetKeyDown("3") && mouseState != Mousestate.Pointspell)
            {
                nextSpellId = 2;
                myGame.tryTargetSpellCast(nextSpellId);
            }
            if (Input.GetKeyDown("4") && mouseState != Mousestate.Pointspell)
            {
                nextSpellId = 3;
                myGame.tryTargetSpellCast(nextSpellId);
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
