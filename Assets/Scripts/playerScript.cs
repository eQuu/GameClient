using UnityEngine;
using System.Collections;

public class playerScript : MonoBehaviour {

    private uint listPosition;

    //Die hier muessen wahrscheinlich zum Character
    //private float posX, posY, posZ;
    //private float rotX, rotY, rotZ;
    //private float movespeed;


    public uint getListPos()
    {
        return this.listPosition;
    }

    public void setListPos(uint newPos)
    {
        this.listPosition = newPos;
    }

    public GameObject getCharacter()
    {
        return this.gameObject;
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
