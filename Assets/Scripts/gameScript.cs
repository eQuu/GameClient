using UnityEngine;
using System.Collections;

public class gameScript : MonoBehaviour {

    enum Command : byte
    {
        Disconnect = 0,
        Connect = 1,
        Chat = 2,
        Move = 3,
        PositionInList = 4,
        CastPointStart = 5,
        CastTargetStart = 6,
        CastFreeStart = 7,
        CastPointEnd = 8,
        CastTargetEnd = 9,
        CastFreeEnd = 10,
        DealDamage = 11,
        DealHeal = 12,
        OnlineCheck = 13,
        DrainMana = 14,
        GiveMana = 15,
        ChangeTarget = 16,
    };

    //Network
    public networkScript myNetwork;
    private uint myPosInList;
    private uint myPlayerId;
    //Game
    public GameObject cubePrefab;
    public GameObject playerPrefab;
    public cameraScript myCamera;
    public uiScript myUiScript;
    private float oldSnapshot, newSnapshot;
    private Command recCommand;
    private uint recPlayer;
    private float recX, recY, recZ, recRotW, recRotX, recRotY, recRotZ;
    private playerScript[] playerList;
    private Vector3 newPosition;
    private Quaternion newRotation;
    uint recPlayerPosInList;
    private Spell[] spellList;

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
                myUiScript.showChatMessage(recPlayerPosInList.ToString(), splitMessage[2]);
                break;
            case Command.Move:
                //Ein Spieler hat sich bewegt
                recX = float.Parse(splitMessage[2]);
                recY = float.Parse(splitMessage[3]);
                recZ = float.Parse(splitMessage[4]);
                recRotW = float.Parse(splitMessage[5]);
                recRotX = float.Parse(splitMessage[6]);
                recRotY = float.Parse(splitMessage[7]);
                recRotZ = float.Parse(splitMessage[8]);
                movePlayer(recPlayerPosInList, recX, recY, recZ, recRotW, recRotX, recRotY, recRotZ);
                break;
            case Command.PositionInList:
                //Mir wird meine Nummer in der Spielerliste gesagt
                myPosInList = recPlayerPosInList;
                myPlayerId = uint.Parse(splitMessage[2]);
                //TODO: Hier dann den playerPrefab instantiieren
                joinWorld();
                break;
            case Command.CastPointEnd:
                //Ein AOE-Spell wurde fertig gecastet
                recX = float.Parse(splitMessage[2]);
                recY = float.Parse(splitMessage[3]);
                recZ = float.Parse(splitMessage[4]);
                //TODO: Hier auf verschiedene Spell reagieren, wtf wie macht man das --> drueber nachdenken
                castCubeSummon(recX, recY, recZ);
                //TODO: Hier wird erstmal fix 20 Mana abgezogen (hartcoded, das geht gar nicht, schaem dich programmierer!)
                playerList[recPlayerPosInList].reduceCurrentMana(20);
                //Wenn wir den Spell gecastet haben, dann setzen wir unseren spell auf CD, ist noch heftig work in progress, aber erstmal nur zum vorzeigen
                if (recPlayerPosInList == myPosInList)
                {
                    myUiScript.setSpellCooldown();
                }
                break;
            case Command.ChangeTarget:
                //Mir wird das target von jemanden gesagt
                if (splitMessage[2] == "null")
                {
                    playerList[recPlayerPosInList].setTarget(null);
                } else
                {
                    uint newTarget = uint.Parse(splitMessage[2]);
                    playerList[recPlayerPosInList].setTarget(playerList[newTarget]);
                }
                break;
            default:
                break;
        }
    }

    private void castCubeSummon(float recX, float recY, float recZ)
    {
        Debug.Log("SPEEEEELLL");
        Vector3 summonPos = new Vector3(recX, recY, recZ);
        cubePrefab = Instantiate(cubePrefab, summonPos, Quaternion.AngleAxis(90f, Vector3.up));
    }

    public int checkValidTarget()
    {
        if (myUiScript.getTarget() != null)
        {
            return 0;
        }
        return 1;
    }

    public void setTarget(uint playerInList, playerScript newTarget)
    {
        playerList[playerInList].setTarget(newTarget);
        Debug.Log("hier komm ich an1: " + playerInList + "," + myPosInList);
        if (playerInList == myPosInList)
        {
            Debug.Log("hier komm ich an2");
            myUiScript.setTarget(newTarget);
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

    private void movePlayer(uint recPlayerPosInList, float recX, float recY, float recZ, float recRotW, float recRotX, float recRotY, float recRotZ)
    {
        newPosition = new Vector3(recX, recY, recZ);
        newRotation = new Quaternion(recRotX, recRotY, recRotZ, recRotW);
        playerList[recPlayerPosInList].setNewPos(newPosition, newRotation);
    }

    private void removePlayerFromList(uint recPlayerPosInList)
    {
        Destroy(playerList[recPlayerPosInList].gameObject);
        playerList[recPlayerPosInList] = null;
    }

    private void addPlayerToList(uint recPlayerPosInList)
    {
        Vector3 spawnPos = new Vector3(47, 25, 9);
        GameObject newPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerScript newPlayerScript = newPlayer.GetComponent<playerScript>();
        newPlayerScript.setListPos(recPlayerPosInList);
        if (newPlayerScript == null)
        {
            Debug.Log(newPlayerScript.getListPos());
        }
        playerList[recPlayerPosInList] = newPlayerScript;
    }

    private void joinWorld()
    {
        //Wir haben eine Nummer vom Server zugewiesen bekommen und betreten die Welt
        Vector3 spawnPos = new Vector3(47, 25, 9);
        GameObject newPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        newPlayer.AddComponent<inputScript>();
        inputScript newInputScript = newPlayer.GetComponent<inputScript>();
        //Dem Inputscript alle Werte geben
        newInputScript.initiate(this, this.myNetwork, this.myCamera, this.myUiScript, newPlayer.GetComponent<Rigidbody>(), newPlayer.GetComponent<Animator>());
        playerScript myPlayerScript = newPlayer.GetComponent<playerScript>();
        myPlayerScript.setMainPlayer(true);
        //Uns selbst in die Liste der Spieler aufnehmen
        playerList[myPosInList] = myPlayerScript;
        //Dem HUD alle Werte geben
        this.myUiScript.initiate(myPlayerScript);
        //Der Kamera alle Werte geben
        myCamera.myPlayer = newPlayer;
    }

    // Use this for initialization
    void Start () {
        Application.runInBackground = true;
        //TODO: Erstmal nur 10 player erlauben
        playerList = new playerScript[10];
        spellList = new Spell[4];
        spellList[0] = new Cube();
        spellList[1] = new Fireblast();
        spellList[2] = new Heal();
        spellList[3] = new Innervate();
    }

	// Update is called once per frame
	void Update () {

    }
}
