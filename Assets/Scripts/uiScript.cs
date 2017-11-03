using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class uiScript : MonoBehaviour {

    public GameObject castIndicator;

    private bool isInitiated = false;
    private playerScript myPlayer;
    private Transform selfImage;
    private Transform selfHealth;
    private Transform selfHealthBar;
    private Transform selfMana;
    private Transform selfManaBar;
    private playerScript myTarget;
    private Transform targetImage;
    private Transform targetHealthBar;
    private Transform targetHealth;
    private Transform targetManaBar;
    private Transform targetMana;
    private spellBarScript mySpellBar;
    private chatScript myChat;
    private Text errText;
    private Stopwatch errWatch;
    private int errDuration = 2000;

    public void initiate(playerScript player)
    {
        this.myPlayer = player;
        isInitiated = true;
    }

    // Use this for initialization
    void Start () {
        selfImage = transform.GetChild(0);
        targetImage = transform.GetChild(1);
        selfHealthBar = selfImage.transform.GetChild(0);
        selfHealth = selfImage.transform.GetChild(1);
        selfManaBar = selfImage.transform.GetChild(2);
        selfMana = selfImage.transform.GetChild(3);
        targetHealthBar = targetImage.transform.GetChild(0);
        targetHealth = targetImage.transform.GetChild(1);
        targetManaBar = targetImage.transform.GetChild(2);
        targetMana = targetImage.transform.GetChild(3);
        mySpellBar = transform.GetChild(2).GetComponent<spellBarScript>();
        myChat = transform.GetChild(3).GetComponent<chatScript>();
        errText = transform.GetChild(4).GetComponent<Text>();
        errWatch = new Stopwatch();
    }
	
	// Update is called once per frame
	void Update () {
        if (isInitiated)
        {
            selfHealth.GetComponent<Text>().text = myPlayer.getCurrentHealth().ToString();
            selfHealthBar.localScale = new Vector3((myPlayer.getCurrentHealth()*1.0f/ myPlayer.getMaxHealth()*1.0f), 1, 1);
            selfMana.GetComponent<Text>().text = myPlayer.getCurrentMana().ToString();
            selfManaBar.localScale = new Vector3((myPlayer.getCurrentMana() * 1.0f / myPlayer.getMaxMana() * 1.0f), 1, 1);
        }
        if (targetImage.gameObject.activeSelf)
        {
            targetHealth.GetComponent<Text>().text = myTarget.getCurrentHealth().ToString();
            targetHealthBar.localScale = new Vector3((myTarget.getCurrentHealth() * 1.0f / myTarget.getMaxHealth() * 1.0f), 1, 1);
            targetMana.GetComponent<Text>().text = myTarget.getCurrentMana().ToString();
            targetManaBar.localScale = new Vector3((myTarget.getCurrentMana() * 1.0f / myTarget.getMaxMana() * 1.0f), 1, 1);
        }

        //ErrText handlen
        if (errWatch.ElapsedMilliseconds > errDuration)
        {
            fadeErrMessage();
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
            this.myTarget = null;
            targetImage.gameObject.SetActive(false);
        }
    }

    public playerScript getTarget()
    {
        return this.myTarget;
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

    private void fadeErrMessage()
    {
        Color newColor = new Color(errText.material.color.r, errText.material.color.g, errText.material.color.b, errText.material.color.a - 0.01f);
        if (newColor.a <= 0f)
        {
            errText.text = "";
            newColor = new Color(errText.material.color.r, errText.material.color.g, errText.material.color.b, 1);
            errWatch.Stop();
            errWatch.Reset();
        }
        errText.material.color = newColor;
    }

    public void showErrMessage(string msg, int durationMS)
    {
        errDuration = durationMS;
        errWatch.Reset();
        errWatch.Start();
        errText.text = msg;
        Color newColor = new Color(errText.material.color.r, errText.material.color.g, errText.material.color.b, 1);
        errText.material.color = newColor;
    }

    public void scrollChat(int direction)
    {
        myChat.scrollChat(direction);
    }

    public void setMouseOverChat(bool newVal)
    {
        myChat.setMouseOver(newVal);
    }

    public bool getMouseOverChat()
    {
        return myChat.getMouseOver();
    }

    public void setSpellCooldown()
    {
        //TODO: Erstmal zum Testen nur den ersten Spell auf CD setzen
        mySpellBar.setCooldown();
    }

    public string sanitizeInput()
    {
        return myChat.sanitizeInput();
    }

    public void showChatMessage(string recPlayer, string recText)
    {
        myChat.addChatMessage(recPlayer + ": "+ recText);
    }
}
