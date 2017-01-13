using UnityEngine;
using System.Collections;

public class playerScript : MonoBehaviour {

    private uint listPosition;
    public GameObject myCharacter;

    //Die hier muessen wahrscheinlich zum Character
    //private float posX, posY, posZ;
    //private float rotX, rotY, rotZ;
    //private float movespeed;

    public playerScript(uint positionInList)
    {
        this.listPosition = positionInList;
    }

    public uint getListPos()
    {
        return this.listPosition;
    }

    public GameObject getCharacter()
    {
        return this.myCharacter;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
