using UnityEngine;
using System.Collections;

public class gamescript : MonoBehaviour {

    public Canvas myHud;
    public AudioClip targetSound;

    private GameObject myPlayer;
    private AudioSource myAudioSource;
    private HUDScript myHudScript;
    private playerscript myPlayerscript;
    private RaycastHit2D clickedOnNothing;


    public void setLocalPlayer(GameObject addedPlayer)
    {
        myPlayer = addedPlayer;
        myPlayerscript = myPlayer.GetComponent<playerscript>();
    }

    public void newTarget(GameObject newTarget)
    {
        myHudScript.newTarget(newTarget);
    }

    public void playSound()
    {
        myAudioSource.clip = targetSound;
        myAudioSource.Play();
    }

    // Use this for initialization
    void Start () {
        myHud = Instantiate(myHud);
        myHudScript = myHud.GetComponent<HUDScript>();
        myAudioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            clickedOnNothing = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (!clickedOnNothing)
            {
                myHudScript.newTarget(null);
            }
        }
    }
}
