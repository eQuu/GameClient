using System;

public class Fireblast : Spell {

    public override void initiate()
    {
        this.spellName = "Fireblast";
        this.spellImage = null;
        this.spellTooltip = "do fire damage and such";
        this.spellDmg = 30;
        this.spellHeal = 0;
        this.spellManacost = 10;
        this.spellCooldown = 3;
        this.spellCasttime = 0;
    }

    public override void onCast()
    {
        
    }
}
