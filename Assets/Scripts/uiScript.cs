using UnityEngine;
using UnityEngine.UI;

public class uiScript : MonoBehaviour {

    public GameObject castIndicator;

    private bool isInitiated = false;
    private playerScript myPlayer;
    private Transform selfImage;
    private Transform selfHealth;
    private Transform selfMana;
    private playerScript myTarget;
    private Transform targetImage;
    private Transform targetHealth;
    private Transform targetMana;
    private spellBarScript mySpellBar;
    private chatScript myChat;

    public void initiate(playerScript player)
    {
        this.myPlayer = player;
        isInitiated = true;
    }

    // Use this for initialization
    void Start () {
        selfImage = transform.GetChild(0);
        targetImage = transform.GetChild(1);
        selfHealth = selfImage.transform.GetChild(0);
        selfMana = selfImage.transform.GetChild(1);
        targetHealth = targetImage.transform.GetChild(0);
        targetMana = targetImage.transform.GetChild(1);
        mySpellBar = transform.GetChild(2).GetComponent<spellBarScript>();
        myChat = transform.GetChild(3).GetComponent<chatScript>();
    }
	
	// Update is called once per frame
	void Update () {
        if (isInitiated)
        {
            selfHealth.GetComponent<Text>().text = myPlayer.getCurrentHealth().ToString();
            selfMana.GetComponent<Text>().text = myPlayer.getCurrentMana().ToString();
        }
        if (targetImage.gameObject.activeSelf)
        {
            targetHealth.GetComponent<Text>().text = myTarget.getCurrentHealth().ToString();
            targetMana.GetComponent<Text>().text = myTarget.getCurrentMana().ToString();
        }
    }

    public void setTarget(playerScript newTarget)
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

    public void focusChat(bool doFocus)
    {
        myChat.setFocused(doFocus);
    }

    public void checkFocus()
    {
        myChat.checkFocus();
    }

    public bool chatIsFocused()
    {
        return myChat.isFocused();
    }

    public void setSpellCooldown()
    {
        //TODO: Erstmal zum Testen nur den ersten Spell auf CD setzen
        mySpellBar.setCooldown();
    }

}
