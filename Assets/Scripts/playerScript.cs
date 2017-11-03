﻿using UnityEngine;
using System.Collections;

public class playerScript : MonoBehaviour {

    private uint listPosition;

    //Die hier muessen wahrscheinlich zum Character
    private Vector3 newPos;
    private Vector3 oldPos;
    private bool isUpToDatePos = false;
    private bool isUpToDateRot = false;
    private Quaternion newRot;
    private Quaternion oldRot;
    private bool isMainPlayer = false;
    private int currentMana, currentHealth;
    private int maximumMana, maximumHealth;
    private Animator myAnimator;
    private float movespeed = 4;
    private Spell[] spellList;
    private playerScript target = null;

    public uint getListPos()
    {
        return this.listPosition;
    }

    public void setNewPos(Vector3 newPos, Quaternion newRot)
    {
        this.newPos = newPos;
        this.newRot = newRot;
        this.isUpToDatePos = false;
        this.isUpToDateRot = false;
    }

    public void setTarget(playerScript newTarget)
    {
        this.target = newTarget;
    }

    public playerScript getTarget()
    {
        return this.target;
    }

    public void setListPos(uint newPos)
    {
        this.listPosition = newPos;
    }

    public void setMainPlayer(bool newVal)
    {
        this.isMainPlayer = newVal;
    }

    public int getCurrentHealth()
    {
        return this.currentHealth;
    }

    public int getMaxHealth()
    {
        if (this.maximumHealth == 0)
        {
            return 100;
        }
        else
        {
            return this.maximumHealth;
        }
    }

    public int getCurrentMana()
    {
        return this.currentMana;
    }

    public int getMaxMana()
    {
        if (this.maximumMana == 0) {
            return 100;
        } else
        {
            return this.maximumMana;
        }
        
    }

    public void reduceCurrentMana(int newVal)
    {
        if (newVal > this.currentMana)
        {
            this.currentMana = 0;
        } else
        {
            this.currentMana = this.currentMana - newVal;
        }
    }

    public void reduceCurrentHealth(int newVal)
    {
        if (newVal > this.currentHealth)
        {
            this.currentHealth = 0;
        }
        else
        {
            this.currentHealth = this.currentHealth - newVal;
        }
    }

    public void increaseCurrentHealth(int newVal)
    {
        if (newVal + currentHealth > this.maximumHealth)
        {
            this.currentHealth = this.maximumHealth;
        }
        else
        {
            this.currentHealth = this.currentHealth + newVal;
        }
    }

    public void increaseCurrentMana(int newVal)
    {
        if (newVal + currentMana > this.maximumMana)
        {
            this.currentMana = this.maximumMana;
        }
        else
        {
            this.currentMana = this.currentMana + newVal;
        }
    }

    public float getMovespeed()
    {
        return this.movespeed;
    }

    public GameObject getGameObject()
    {
        return this.gameObject;
    }

    private Vector3 lerp(float dt)
    {
        //Funktion um zwischen zwei Positionen zu interpolieren
        //Faktor gibt die Geschwindigkeit an.Hoher Wert fuehrt zu schnellen aber ruckeligen Bewegungen
        //Niedriger Wert fuehrt zu fluessigen aber langsamen Bewegungen
        int factor = 4;
        return oldPos + (factor * ((newPos - oldPos) * dt));
    }

    private void interpolate(float dt)
    {
        Vector3 changedPos = Vector3.Lerp(oldPos, newPos, 0.1f);
        Quaternion changedRot = Quaternion.Slerp(oldRot, newRot, 0.2f);

        //Position interpolieren
        if ((Mathf.Abs(changedPos.x - newPos.x) <= 0.005f) && (Mathf.Abs(changedPos.z - newPos.z) <= 0.005f))
        {
            this.isUpToDatePos = true;
            this.oldPos = newPos;
            this.transform.position = newPos;
        } else
        {
            this.oldPos = changedPos;
            this.transform.position = changedPos;
        }

        //Rotation interpolieren
        if (Mathf.Abs(changedRot.w - newRot.w) <= 0.001f)
        {
            this.isUpToDateRot = true;
            this.oldRot = newRot;
            this.transform.rotation = newRot;
        }
        else
        {
            this.oldRot = changedRot;
            this.transform.rotation = changedRot;
        }

        //Animation anpassen
        if (isUpToDatePos && isUpToDateRot)
        {
            myAnimator.SetBool("isWalking", false);
        }
    }

    // Use this for initialization
    void Start () {
        this.myAnimator = gameObject.GetComponent<Animator>();
        maximumMana = 100;
        currentMana = maximumMana;
        maximumHealth = 100;
        currentHealth = maximumHealth;
}
	
	// Update is called once per frame
	void Update () {
        float dt = Time.deltaTime;
	    //Alle Spieler interpolieren ausser den eigenen
        if (!this.isMainPlayer && (!isUpToDatePos || !isUpToDateRot))
        {
            //interpolieren
            myAnimator.SetBool("isWalking", true);
            interpolate(dt);
        }
	}
}
