using UnityEngine;
using System.Collections;

public class FreeCam : MonoBehaviour
{
    public float speedNormal = 3f;
    public float speedFast = 45f;
    public float mouseSensX = 2f;
    public float mouseSensY = 2f;
    public float minPosY;
    public float maxPosY;
    float rotY;
    float speed;

    void Start()
    {
        //Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //Cursor.visible = true;
        }
        //rotY += Input.GetAxis("Mouse ScrollWheel") * mouseSensY;
        //rotY = Mathf.Clamp(rotY, -80f, 80f);
        //float rotX = transform.localEulerAngles.y + (Input.GetAxis("Mouse ScrollWheel") * mouseSensX);
        //transform.localEulerAngles = new Vector3(0f, rotX, 0f);
        if (Input.GetMouseButton(2))
        {
            //float rotX = transform.localEulerAngles.y + (Input.GetAxis("Mouse X") * mouseSensX);
            /*rotY += Input.GetAxis("Mouse ScrollWheel") * mouseSensY;
            rotY = Mathf.Clamp(rotY, -80f, 80f);
            transform.localEulerAngles = new Vector3(-rotY, 0f, 0f);*/
            //Cursor.visible = false;
        }
        float forward = Input.GetAxis("Vertical");
        float side = Input.GetAxis("Horizontal");
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (forward != 0f)
        {
            if (Input.GetKey(KeyCode.LeftShift)) speed = speedFast;
            else speed = speedNormal;
            Vector3 vect = new Vector3(0f, 0f, forward * speed * Time.deltaTime);
            transform.localPosition += vect;
            //transform.localPosition += transform.localRotation * vect;
        }
        if (side != 0f)
        {
            if (Input.GetKey(KeyCode.LeftShift)) speed = speedFast;
            else speed = speedNormal;
            Vector3 vect = new Vector3(side * speed * Time.deltaTime, 0f, 0f);
            transform.localPosition += vect;
            //transform.localPosition += transform.localRotation * vect;
        }
        if (zoom != 0f)
        {
            speed = speedNormal;
            Vector3 vect = new Vector3(0f, -zoom * speed * Time.deltaTime * 10, 0f);
            if (transform.position.y <= maxPosY && transform.position.y >= minPosY)
                transform.localPosition += vect;
            if (transform.position.y > maxPosY)
            {
                Vector3 temp = transform.position;
                temp.y = maxPosY;
                transform.position = temp;
            }
            if (transform.position.y < minPosY)
            {
                Vector3 temp = transform.position;
                temp.y = minPosY;
                transform.position = temp;
            }
            //transform.localPosition += transform.localRotation * vect;
        }
    }
}