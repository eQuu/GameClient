﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class chatScript : MonoBehaviour {

    private bool chatIsFocused = false;
    private InputField myChatInput;

    // Use this for initialization
    void Start () {
        myChatInput = transform.GetChild(0).GetComponent<InputField>();
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
}
