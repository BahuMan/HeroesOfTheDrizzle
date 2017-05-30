using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCameraControl : MonoBehaviour {

    [SerializeField]
    float ScrollSpeed = 15;
    [SerializeField]
    float ScrollEdge = 0.01f;
 
    [SerializeField]
    float PanSpeed = 10;

    //[SerializeField]
    //Vector2 ZoomRange = new Vector2(-5,5);
    //[SerializeField]
    //float CurrentZoom = 0;
    //[SerializeField]
    //float ZoomSpeed = 1;
    [SerializeField]
    float RotationSpeed = 1;
    //[SerializeField]
    //float ZoomRotation = 1;
    [SerializeField]
    bool ScrollAtEdges = true;
 
    private Vector3 InitPos;
    private Vector3 InitRotation;
 
	void LateUpdate () {
        //PAN
        if (Input.GetMouseButton(1))
        {
            //(Input.mousePosition.x - Screen.width * 0.5)/(Screen.width * 0.5)

            transform.Translate(Vector3.right * Time.deltaTime * PanSpeed * (Input.mousePosition.x - Screen.width * 0.5f) / (Screen.width * 0.5f), Space.World);
            transform.Translate(Vector3.forward * Time.deltaTime * PanSpeed * (Input.mousePosition.y - Screen.height * 0.5f) / (Screen.height * 0.5f), Space.World);

        }
        else
        {
            if (Input.GetAxis("Horizontal") > .3f || (ScrollAtEdges && Input.mousePosition.x >= Screen.width * (1 - ScrollEdge)))
            {
                transform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed, Space.World);
            }
            else if (Input.GetAxis("Horizontal") < -.3f || (ScrollAtEdges && Input.mousePosition.x <= Screen.width * ScrollEdge))
            {
                transform.Translate(Vector3.right * Time.deltaTime * -ScrollSpeed, Space.World);
            }

            if (Input.GetAxis("Vertical") > .3f || (ScrollAtEdges && Input.mousePosition.y >= Screen.height * (1 - ScrollEdge)))
            {
                transform.Translate(Vector3.forward * Time.deltaTime * ScrollSpeed, Space.World);
            }
            else if (Input.GetAxis("Vertical") < -.3f || (ScrollAtEdges && Input.mousePosition.y <= Screen.height * ScrollEdge))
            {
                transform.Translate(Vector3.forward * Time.deltaTime * -ScrollSpeed, Space.World);
            }
        }

        //ZOOM IN/OUT

        //CurrentZoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000 * ZoomSpeed;
        //CurrentZoom = Mathf.Clamp(CurrentZoom, ZoomRange.x, ZoomRange.y);
        //transform.Translate(Vector3.up * (transform.position.y - (InitPos.y + CurrentZoom)) * 0.1f);
        //transform.eulerAngles -= Vector3.right * (transform.eulerAngles.x - (InitRotation.x + CurrentZoom * ZoomRotation)) * 0.1f;

        //ROTATION
        //Use Mouse 1 + Ctrl key to rotate the camera around the Y axis
        if (Input.GetMouseButton(0) && Input.GetKey("left ctrl")){
            float requestedRotation = Input.mousePosition.x - Screen.width / 2.0f;
            transform.Rotate(0, requestedRotation * RotationSpeed * Time.deltaTime, 0, Space.World);
        }
    }
}
