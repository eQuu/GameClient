using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDScript : MonoBehaviour {

    private GameObject myTarget;
    private Image myTargetImage;
    private gamescript myGameScript;

    public void newTarget(GameObject newTarget)
    {
        myTarget = newTarget;
        myTargetImage = transform.Find("Target").gameObject.GetComponent<Image>();
        if (myTarget == null)
        {
            if (myTargetImage.enabled == true)
            {
                myGameScript.playSound();
                myTargetImage.enabled = false;
            }
            
        } else
        {
            if (myTargetImage.enabled == false)
            {
                myGameScript.playSound();
                myTargetImage.enabled = true;
            }
        }
        
    }

	// Use this for initialization
	void Start () {
        myGameScript = GameObject.Find("Game").GetComponent<gamescript>();
	}
	
	// Update is called once per frame
	void Update () {

	}
}
