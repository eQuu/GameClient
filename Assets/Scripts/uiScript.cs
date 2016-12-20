using UnityEngine;
using System.Collections;

public class uiScript : MonoBehaviour {

    private Transform myTarget;
    private Transform targetImage;
    private Transform selfImage;
	// Use this for initialization
	void Start () {
        selfImage = transform.GetChild(0);
        targetImage = transform.GetChild(1);
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void setTarget(Transform newTarget)
    {
        if (newTarget != null)
        {
            this.myTarget = newTarget;
            targetImage.gameObject.SetActive(true);
        } else
        {
            targetImage.gameObject.SetActive(false);
        }

    }
}
