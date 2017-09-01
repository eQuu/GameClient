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
    private uint recTargetPosInList;
    private uint recSpellPosInList;
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
                recSpellPosInList = uint.Parse(splitMessage[5]);
                //TODO: Hier auf verschiedene Spell reagieren, wtf wie macht man das --> drueber nachdenken
                castCubeSummon(recX, recY, recZ);
                //TODO: Hier wird Mana abgezogen (hartcoded, das geht gar nicht, schaem dich programmierer!)
                playerList[recPlayerPosInList].reduceCurrentMana(spellList[recSpellPosInList].spellManacost);
                //Wenn wir den Spell gecastet haben, dann setzen wir unseren spell auf CD, ist noch heftig work in progress, aber erstmal nur zum vorzeigen
                if (recPlayerPosInList == myPosInList)
                {
                    myUiScript.setSpellCooldown();
                }
                break;
            case Command.CastTargetEnd:
                recTargetPosInList = uint.Parse(splitMessage[2]);
                recSpellPosInList = uint.Parse(splitMessage[3]);
                //TODO: Hier kommt eigtl onCast() vom Spell hin, aber erstmal hardcoden
                wipTargetCast(recTargetPosInList, recSpellPosInList);
                playerList[recPlayerPosInList].reduceCurrentMana(spellList[recSpellPosInList].spellManacost);
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

    //TODO: Das hier wird zu onCast() vom Spell
    private void wipTargetCast(uint target, uint spell)
    {
        playerScript spellTarget = playerList[target];
        Spell castedSpell = spellList[spell];
        switch(spell) {
            case 1:
                Debug.Log("DAMAGE: " + castedSpell.spellDmg);
                spellTarget.reduceCurrentHealth(castedSpell.spellDmg);
                break;
            case 2:
                spellTarget.increaseCurrentHealth(castedSpell.spellHeal);
                break;
            case 3:
                spellTarget.increaseCurrentMana(castedSpell.spellHeal);
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

    public void setTarget(uint playerInList, playerScript newTarget)
    {
        playerList[playerInList].setTarget(newTarget);
        if (playerInList == myPosInList)
        {
            myUiScript.setTarget(newTarget);
        }
    }

    public void setTarget(playerScript newTarget)
    {
        playerList[myPosInList].setTarget(newTarget);
        myUiScript.setTarget(newTarget);
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
        myPlayerScript.setListPos(myPosInList);
        //Uns selbst in die Liste der Spieler aufnehmen
        playerList[myPosInList] = myPlayerScript;
        //Dem HUD alle Werte geben
        this.myUiScript.initiate(myPlayerScript);
        //Der Kamera alle Werte geben
        myCamera.myPlayer = newPlayer;
    }

    public playerScript getOwnPlayer()
    {
        return playerList[myPosInList];
    }

    public void tryPointSpellCast(uint spellId, Vector3 point)
    {
        //TODO: Out of Range und Cooldown
        if (getOwnPlayer().getCurrentMana() >= spellList[spellId].spellManacost && getOwnPlayer().getCurrentHealth() >= spellList[spellId].spellHPcost)
        {
            //TODO: CDs von Spells muessen auf den Spieler bezogen sein: spellList[spellId].setCD();
            myNetwork.sendMessage("5;" + myPlayerId + ";" + myPosInList + ";" + point.x + ";" + point.y + ";" + point.z + ";" + spellId);
        } else
        {
            myUiScript.showErrMessage("Irgendein Error! Kein Plan", 1000);
        }
    }

    public void tryTargetSpellCast(uint spellId)
    {
        //TODO: Out of Range und Cooldown
        playerScript target = playerList[myPosInList].getTarget();
        if (target != null) {
            uint targetInList = target.getListPos();
            if (getOwnPlayer().getCurrentMana() >= spellList[spellId].spellManacost && getOwnPlayer().getCurrentHealth() >= spellList[spellId].spellHPcost)
            {
                myNetwork.sendMessage("6;" + myPlayerId + ";" + myPosInList + ";" + targetInList + ";" + spellId);
            }else
            {
                myUiScript.showErrMessage("Irgendein Error! Kein Plan", 1000);
            }
        }else
        {
            myUiScript.showErrMessage("Irgendein Error! Kein Plan", 1000);
        }
    }

    public void tryFreeSpellCast(uint spellId)
    {
        //TODO: Out of Range und Cooldown
        if (getOwnPlayer().getCurrentMana() >= spellList[spellId].spellManacost && getOwnPlayer().getCurrentHealth() >= spellList[spellId].spellHPcost)
        {
            myNetwork.sendMessage("7;" + myPlayerId + ";" + myPosInList + ";" + spellId);
        }else
        {
            myUiScript.showErrMessage("Irgendein Error! Kein Plan", 1000);
        }
    }

    // Use this for initialization
    void Start() {
        Application.runInBackground = true;
        //TODO: Erstmal nur 10 player erlauben
        playerList = new playerScript[10];
        spellList = new Spell[4];
        Spell newSpell = ScriptableObject.CreateInstance<Cube>();
        newSpell.initiate();
        spellList[0] = newSpell;
        newSpell = ScriptableObject.CreateInstance<Fireblast>();
        newSpell.initiate();
        spellList[1] = newSpell;
        newSpell = ScriptableObject.CreateInstance<Heal>();
        newSpell.initiate();
        spellList[2] = newSpell;
        newSpell = ScriptableObject.CreateInstance<Innervate>();
        newSpell.initiate();
        spellList[3] = newSpell;
    }

	// Update is called once per frame
	void Update () {

    }
}
