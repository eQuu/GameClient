using System;

public class Innervate : Spell
{

    public override void initiate()
    {
        this.spellName = "Innervate";
        this.spellImage = null;
        this.spellTooltip = "Kotol giff me mana!";
        this.spellDmg = 0;
        this.spellHeal = 60;
        this.spellManacost = 10;
        this.spellCooldown = 3;
        this.spellCasttime = 0;
    }

    public override void onCast()
    {
        
    }
}