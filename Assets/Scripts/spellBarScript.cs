using UnityEngine;
using UnityEngine.UI;

public class spellBarScript : MonoBehaviour {

    //TODO: der CD kommt natuerlich aus der spellList
    private float cd = 4.5f;
    private float currentCD = 0;
    private Image testCDImage;

    private Spell[] spellList;
    private Transform spellImageList;

    // Use this for initialization
    void Start () {
        //TODO: Hardcoded, nicht schoen aber vorzeigbar.
        spellImageList = transform.GetChild(0);
        testCDImage = spellImageList.GetChild(1).GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        if (currentCD > 0)
        {
            if (Time.deltaTime > currentCD)
            {
                currentCD = 0;
                testCDImage.fillAmount = 0;
            }
            else
            {
                currentCD = currentCD - Time.deltaTime;
                testCDImage.fillAmount = 100 / cd * currentCD / 100;
            }
        }
	}

    public void setCooldown()
    {
        currentCD = cd;
    }
}
