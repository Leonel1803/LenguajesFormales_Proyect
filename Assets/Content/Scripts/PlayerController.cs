using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    //Character
    Transform playerTr;
    Rigidbody playerRb;
    Animator playerAnim;

    public GameObject pistol;
    public GameObject rifle;

    public float playerSpeed = 0f;

    private bool hasPistol = false;
    private bool isPvP = false;
    private bool hasRifle = false;

    private Vector2 newDirection;

    //Camera
    public Transform cameraAxis;
    public Transform cameraTrack;
    private Transform cameraC;

    private float rotY = 0f;
    private float rotX = 0f;

    public float camRotSpeed = 200f;
    public float minAngle = -25f;
    public float maxAngle = 45f;
    public float cameraSpeed = 200f;

    //Enemy
    Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        playerTr = this.transform;
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponentInChildren<Animator>();

        cameraC = Camera.main.transform;

        isPvP = true;

        Cursor.lockState = CursorLockMode.Locked;

        enemy = GameObject.Find("/WashedFish").GetComponent<Enemy>();
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

        cameraC.position = Vector3.Lerp(cameraC.position, cameraTrack.position, cameraSpeed * time);
        cameraC.rotation = Quaternion.Lerp(cameraC.rotation, cameraTrack.rotation, cameraSpeed * time);
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
        WeaponSelector();

        playerAnim.SetFloat("X", newDirection.x);
        playerAnim.SetFloat("Y", newDirection.y);
        playerAnim.SetBool("holdPistol", hasPistol);
        playerAnim.SetBool("holdRifle", hasRifle);

        playerAnim.SetLayerWeight(0, isPvP ? 1 : 0);
        playerAnim.SetLayerWeight(1, hasPistol ? 1 : 0);
        playerAnim.SetLayerWeight(2, hasRifle ? 1 : 0);

        if (isPvP)
        {
            //var atm = enemy.getComponent<AttributeManager>();
            enemy.TakeDamage(5);
            if (Input.GetKeyDown(KeyCode.F)){
                playerAnim.Play("Kick");
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                switch(Random.Range(1, 4))
                {
                    case 1:
                        playerAnim.Play("Left Punch");
                        break;
                    case 2:
                        playerAnim.Play("Right Punch");
                        break;
                    case 3:
                        playerAnim.Play("Elbow Punch");
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void WeaponSelector()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isPvP = true;
            hasPistol = false;
            hasRifle = false;

            playerAnim.Play("Change Weapon");
            pistol.SetActive(hasPistol);
            rifle.SetActive(hasRifle);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isPvP = false;
            hasPistol = true;
            hasRifle = false;

            playerAnim.Play("Change Weapon");
            rifle.SetActive(hasRifle);
            pistol.SetActive(hasPistol);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            isPvP = false;
            hasPistol = false;
            hasRifle = true;

            playerAnim.Play("Change Weapon");
            pistol.SetActive(hasPistol);
            rifle.SetActive(hasRifle);
        }
    }
}
