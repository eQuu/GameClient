using UnityEngine;
using System.Collections;

public class gameScript : MonoBehaviour {

    enum Command : byte
    {
        Disconnect = 0,
        Connect = 1,
        Chat = 2,
        Move = 3,
        PositionInList = 4
    };

    //Network
    public networkScript myNetwork;
    private uint myPosInList;
    private uint myPlayerId;
    //Game
    public GameObject playerPrefab;
    public cameraScript myCamera;
    public uiScript myUiScript;
    private Command recCommand;
    private uint recPlayer;
    private float recX, recY, recZ;
    private playerScript[] playerList;
    private Vector3 newPosition;
    private string outMessage;
    uint recPlayerPosInList;

    public void processMessage(string[] splitMessage)
    {
        recCommand = (Command)byte.Parse(splitMessage[0]);
        recPlayerPosInList = uint.Parse(splitMessage[1]);

        switch (recCommand)
        {
            case Command.Disconnect:
                //Ein Spieler ist disconnected
                removePlayerFromList(recPlayerPosInList);
                break;
            case Command.Connect:
                //Ein Spieler ist connected
                addPlayerToList(recPlayerPosInList);
                break;
            case Command.Chat:
                //Ein Spieler will eine Chatnachricht verschicken
                break;
            case Command.Move:
                //Ein Spieler hat sich bewegt
                recX = float.Parse(splitMessage[2]);
                recY = float.Parse(splitMessage[3]);
                recZ = float.Parse(splitMessage[4]);
                Debug.Log("player " + recPlayerPosInList + " moved: " + recX + " " + recY + " " + recZ);
                movePlayer(recPlayerPosInList, recX, recY, recZ);
                break;
            case Command.PositionInList:
                //Mir wird meine Nummer in der Spielerliste gesagt
                myPosInList = recPlayerPosInList;
                myPlayerId = uint.Parse(splitMessage[2]);
                //TODO: Hier dann den playerPrefab instantiieren
                joinWorld();
                break;
            default:
                break;
        }
    }

    public uint getMyPosInList()
    {
        return this.myPosInList;
    }

    public uint getMyPlayerId()
    {
        return this.myPlayerId;
    }

    private void movePlayer(uint recPlayerPosInList, float recX, float recY, float recZ)
    {
        newPosition = new Vector3(recX, recY, recZ);
        playerList[recPlayerPosInList].getCharacter().transform.position = newPosition;
    }

    private void removePlayerFromList(uint recPlayerPosInList)
    {
        playerList[recPlayerPosInList] = null;
    }

    private void addPlayerToList(uint recPlayerPosInList)
    {
        playerScript playerToAdd = new playerScript(recPlayerPosInList);
        playerList[recPlayerPosInList] = playerToAdd;
    }

    private void joinWorld()
    {
        //Wir haben eine Nummer vom Server zugewiesen bekommen und betreten die Welt
        Vector3 spawnPos = new Vector3(47, 25, 9);
        GameObject newPlayer = (GameObject)Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        inputScript newPlayerScript = newPlayer.GetComponent<inputScript>();
        //Dem Player alle Werte geben
        newPlayerScript.myGame = this;
        newPlayerScript.myNetwork = this.myNetwork;
        newPlayerScript.myCameraScript = this.myCamera;
        newPlayerScript.myUiScript = this.myUiScript;
        //Der Kamera alle Werte geben
        myCamera.myPlayer = newPlayer;
    }

    // Use this for initialization
    void Start () {
    }

	// Update is called once per frame
	void Update () {
        //TODO: Das hier wird ausgefuehrt, wenn der Server eine Position liefert
        if (Input.GetKeyDown("k"))
        {

        }
    }
}
