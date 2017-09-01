using System;

public class Heal : Spell
{
    public override void initiate()
    {
        this.spellName = "Heal";
        this.spellImage = null;
        this.spellTooltip = "heal your target for amounts. How much? Figure it out.";
        this.spellDmg = 0;
        this.spellHeal = 15;
        this.spellManacost = 10;
        this.spellHPcost = 0;
        this.spellCooldown = 1;
        this.spellCurrCooldown = 0;
        this.spellCasttime = 0;
    }

    public override void onCast()
    {

    }

    public override void setCD()
    {
        this.spellCurrCooldown = spellCooldown;
    }
}