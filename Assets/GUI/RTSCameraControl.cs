using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RTSCameraControl : MonoBehaviour {

    [SerializeField]
    private float ScrollSpeed = 15;
    [SerializeField]
    private float ScrollEdge = 0.01f;
 
    [SerializeField]
    private float PanSpeed = 10;

    [SerializeField]
    private float RotationSpeed = 1;
    [SerializeField]
    private bool ScrollAtEdges = true;

    //if the user is pressing buttons, we won't force the camera somewhere.
    //if the user hasn't pressed a button for a while, we will center the camera on the hero
    private float lastButtonTime = 0;
    [SerializeField]
    private float _ButtonTimeOut = 5;

    //camera smooth movement variables:
    [SerializeField]
    private float _smoothTime = 2f;
    private Vector3 _curVelocity = Vector3.zero;
    private Camera _cam;

    private const int _TerrainMask = 1 << 8;
    private HeroControl _localHero = null;

    public UnityEngine.UI.Text DebugText;

    private void Start()
    {
        _cam = GetComponent<Camera>();
    }

    void LateUpdate () {
        //PAN
        if (Input.GetMouseButton(1))
        {
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

            if (Mathf.Abs(Input.GetAxis("Horizontal")) > .1f || Mathf.Abs(Input.GetAxis("Vertical")) > .1f)
            {
                lastButtonTime = Time.time;
            }

        }

        //ROTATION
        //Use Mouse 1 + Ctrl key to rotate the camera around the Y axis
        if (Input.GetMouseButton(0) && Input.GetKey("left ctrl")){
            float requestedRotation = Input.mousePosition.x - Screen.width / 2.0f;
            transform.Rotate(0, requestedRotation * RotationSpeed * Time.deltaTime, 0, Space.World);
        }

        CenterCameraOnHero();
    }

    private void CenterCameraOnHero()
    {
        if (_localHero == null) return;
        if (lastButtonTime + this._ButtonTimeOut > Time.time) return; //if button was pressed too recently, don't scroll

        Vector3 orig = _cam.ViewportToWorldPoint(new Vector3(.5f, .5f, 0f));
        Ray r = new Ray(orig, transform.forward);
        //Ray r = _cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        RaycastHit hitInfo;
        if (Physics.Raycast(r, out hitInfo, 1000f, _TerrainMask))
        {

            DebugText.text = hitInfo.collider.name;

            //we want to move what we're looking AT, to the position of the hero
            //so first, calculate the offset of the camera vs what we're look at:
            Vector3 cameraOffset = this.transform.position - hitInfo.point;
            cameraOffset.y = transform.position.y;

            //then using smooth damp, move what we're looking AT,
            Vector3 targetLookAt = Vector3.SmoothDamp(hitInfo.point, _localHero.transform.position, ref _curVelocity, _smoothTime, ScrollSpeed);
            targetLookAt.y = 0;

            //and apply the offset to find our camera position:
            this.transform.position = cameraOffset + targetLookAt;

        }
        else
        {
            DebugText.text = "-- nothing --";
        }

    }
    public void SetLocalHero(HeroControl hero)
    {
        _localHero = hero;
    }
}
