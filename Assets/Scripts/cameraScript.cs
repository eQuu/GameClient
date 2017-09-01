using UnityEngine;
using System.Collections;

public class cameraScript : MonoBehaviour {

    public GameObject myPlayer;
    private float distance = 5.0f;
    private float zoomSpeed = 1.0f;
    private float currentX = 180.0f;
    private float currentY = -15.0f;
    const float minY = -70.0f;
    const float maxY = -1.0f;
    private float sensivityX = 4.0f;
    private float sensivityY = 1.0f;
    private Vector3 direction;
    private Vector3 headPos;

    private Quaternion rotation;

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Update is called once per frame
    void LateUpdate () {

        if (myPlayer != null)
        {
            direction = new Vector3(0, 1.75f, distance);
            rotation = Quaternion.Euler(currentY, currentX, 0);
            transform.position = myPlayer.transform.position + rotation * direction;
            headPos = new Vector3(myPlayer.transform.position.x, myPlayer.transform.position.y + 1.75f, myPlayer.transform.position.z);
            transform.LookAt(headPos);
        }
    }

    public void zoomCamera()
    {
        distance = distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if (distance < 1)
        {
            distance = 1;
        }
    }

    public void rotateCamera()
    {
        currentX = currentX + Input.GetAxis("Mouse X");
        currentY = currentY + Input.GetAxis("Mouse Y");

        clampHorizontal();
    }

    public void followRotation(float playerInputX, float playerInputY)
    {
        this.currentX = this.currentX + playerInputX;
        this.currentY = this.currentY + playerInputY;

        clampHorizontal();
    }

    private void clampHorizontal()
    {
        currentY = Mathf.Clamp(currentY, minY, maxY);
    }

}
