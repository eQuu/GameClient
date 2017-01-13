using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class mainMenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void joinGame()
    {
        SceneManager.LoadScene("dbg");
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
