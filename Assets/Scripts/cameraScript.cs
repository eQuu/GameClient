using UnityEngine;
using System.Collections;

public class cameraScript : MonoBehaviour {

    public GameObject myPlayer;
    private float distance = 1.0f;
    private float zoomSpeed = 1.0f;
    private float currentX = -20.0f;
    private float currentY = -15.0f;
    const float minY = -70.0f;
    const float maxY = -1.0f;
    private float sensivityX = 4.0f;
    private float sensivityY = 1.0f;
    private Vector3 direction;

    private Quaternion rotation;

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            currentX = currentX + Input.GetAxis("Mouse X");
            currentY = currentY + Input.GetAxis("Mouse Y");

            currentY = Mathf.Clamp(currentY, minY, maxY);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            distance = distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        } else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            distance = distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        }
    }

    // Update is called once per frame
    void LateUpdate () {

        direction = new Vector3(0, 0, distance);
        rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = myPlayer.transform.position + rotation * direction;
        transform.LookAt(myPlayer.transform.position);

    }

    public void followRotation(float playerInput)
    {
        this.currentX = this.currentX + playerInput;
    }

}
