using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : ScriptableObject {

    public string spellName;
    public Sprite spellImage;
    public string spellTooltip;
    public int spellDmg;
    public int spellHeal;
    public int spellManacost;
    public int spellHPcost;
    public int spellCooldown;
    public int spellCurrCooldown;
    public int spellCasttime;

    public abstract void onCast();
    public abstract void setCD();
    public abstract void initiate();
}
