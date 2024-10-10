using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    //Character
    Transform playerTr;
    Rigidbody playerRb;
    Animator playerAnim;

    public float playerSpeed = 0f;

    private Vector2 newDirection;

    //Camera
    public Transform cameraAxis;
    public Transform cameraTrack;
    private Transform camera;

    private float rotY = 0f;
    private float rotX = 0f;

    public float camRotSpeed = 200f;
    public float minAngle = -45f;
    public float maxAngle = 45f;
    public float cameraSpeed = 200f;

    // Start is called before the first frame update
    void Start()
    {
        playerTr = this.transform;
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponentInChildren<Animator>();

        camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        MoveLogic();
        CameraLogic();
        AnimLogic();
    }

    public void CameraLogic()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float time = Time.deltaTime;

        rotY += mouseY * time * camRotSpeed;
        rotX = mouseX * time * camRotSpeed;

        playerTr.Rotate(0, rotX, 0);

        rotY = Mathf.Clamp(rotY, minAngle, maxAngle);

        Quaternion localRotation = Quaternion.Euler(-rotY, 0, 0);
        cameraAxis.localRotation = localRotation;

        camera.position = Vector3.Lerp(camera.position, cameraTrack.position, cameraSpeed * time);
        camera.rotation = Quaternion.Lerp(camera.rotation, cameraTrack.rotation, cameraSpeed * time);
    }

    public void MoveLogic()
    {
        Vector3 direction = playerRb.velocity;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float time = Time.deltaTime;

        newDirection = new Vector2(moveX, moveZ);

        Vector3 side = playerSpeed * moveX * time * playerTr.right;
        Vector3 forward = playerSpeed * moveZ * time * playerTr.forward;

        Vector3 endDirection = side + forward;

        playerRb.velocity = endDirection;
    }

    public void AnimLogic()
    {
        playerAnim.SetFloat("X", newDirection.x);
        playerAnim.SetFloat("Y", newDirection.y);
    }
}
