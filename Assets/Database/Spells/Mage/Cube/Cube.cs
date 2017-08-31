using System;

public class Cube : Spell
{

    public override void initiate()
    {
        this.spellName = "Cube";
        this.spellImage = null;
        this.spellTooltip = "summon a cube that others will not have. Its all yours!";
        this.spellDmg = 0;
        this.spellHeal = 0;
        this.spellManacost = 20;
        this.spellCooldown = 3;
        this.spellCasttime = 0;
    }

    public override void onCast()
    {

    }
}