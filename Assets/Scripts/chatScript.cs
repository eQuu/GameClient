using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class chatScript : MonoBehaviour {

    private bool chatIsFocused = false;
    private bool mouseOver = false;
    private InputField myChatInput;
    private Text myChatText;
    private String[] chatLines;
    private const int maxChatLines = 20; //Niemals kleiner als 10 setzen!
    private int currentChatLine = 0;

    // Use this for initialization
    void Start () {
        myChatInput = transform.GetChild(0).GetComponent<InputField>();
        myChatText = transform.GetChild(1).GetComponent<Text>();
        chatLines = new String[maxChatLines];

        for (int i = 0; i < maxChatLines; i++)
        {
            chatLines[i] = "";
        }
}
	
    public void setFocused(bool doFocus)
    {
        if (doFocus)
        {
            chatIsFocused = true;
            EventSystem.current.SetSelectedGameObject(myChatInput.gameObject);
        }
        else
        {
            myChatInput.text = "";
            chatIsFocused = false;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public bool isFocused()
    {
        if (myChatInput.isFocused)
        {
            chatIsFocused = true;
            return myChatInput.isFocused;
        }
        else
        {
            return this.chatIsFocused;
        }
    }

    public void setMouseOver(bool doMouseOver)
    {
        this.mouseOver = doMouseOver;
    }

    public bool getMouseOver()
    {
        return this.mouseOver;
    }

    public void scrollChat(int direction)
    {
        if (direction < 0)
        {
            if (currentChatLine == 0)
            {
                //Do nothing
            }else if (chatLines[currentChatLine - 1] == "")
            {
               //Do nothing
            } else
            {
                this.currentChatLine = currentChatLine + direction;
                showChatMessage();
            }
        } else
        {
            this.currentChatLine = currentChatLine + direction;
            if (currentChatLine + 9 > maxChatLines)
            {
                currentChatLine = maxChatLines - 9;
            }
            showChatMessage();
        }
    }

	// Update is called once per frame
	void Update () {

	}

    internal void checkFocus()
    {
        if (EventSystem.current.currentSelectedGameObject != myChatInput.gameObject)
        {
            myChatInput.text = "";
            chatIsFocused = false;
        }
    }

    public string sanitizeInput()
    {
        string sanText = myChatInput.text.Replace(";","<_tr>");
        return sanText;
    }

    private string sanitizeOutput(string recText)
    {
        string sanText = recText.Replace("<_tr>", ";");
        return sanText;
    }

    public void addChatMessage(string message)
    {
        for (int i = 0; i < maxChatLines; i++)
        {
            if (i == maxChatLines - 1)
            {
                chatLines[i] = sanitizeOutput(message);
            } else
            {
                chatLines[i] = chatLines[i + 1];
            }
        }
        currentChatLine = maxChatLines - 9;
        showChatMessage();
    }

    private void showChatMessage()
    {
        int lineCount = 0;
        myChatText.text = "";
        for (int i = currentChatLine; lineCount < 9; i++)
        {

            if (lineCount == 8)
            {
                myChatText.text = myChatText.text + chatLines[i];
            } else
            {
                myChatText.text = myChatText.text + chatLines[i] + "\n";
            }
            lineCount = lineCount + 1;
        }
    }
}
