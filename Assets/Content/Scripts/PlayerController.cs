using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    //Character
    Transform playerTr;
    Rigidbody playerRb;
    Animator playerAnim;

    public GameObject pistol;
    public GameObject rifle;

    public GameObject enemyGO;

    public float playerSpeed = 0f;
    public float health = 100f;
    private float maxHealth;

    private bool hasPistol = false;
    private bool isPvP = false;
    private bool hasRifle = false;

    public Collider foot;
    public Collider rightElbow;
    public Collider leftHand;
    public Collider rightHand;

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
    private Enemy enemy;

    //HpBar
    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        playerTr = this.transform;
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponentInChildren<Animator>();
        enemy = enemyGO.GetComponent<Enemy>();
        cameraC = Camera.main.transform;
        slider = GetComponentInChildren<Slider>();

        isPvP = true;
        maxHealth = health;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(foot.isTrigger + ", " + leftHand.isTrigger + ", " + rightHand.isTrigger + ", " + foot.isTrigger);
        //Debug.Log(health);
        CameraLogic();

        if (health > 0)
        {
            MoveLogic();
            AnimLogic();
        }
    }

    //public void OnTriggerEnter(Collider other)


    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        Collider other = collision.collider;
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "TeletubbieHand")
        {
            //Debug.Log("Entra al if");
            TakeDamage(5);
        }
    }

    public void TakeDamage(float damage = 10)
    {
        health -= damage;
        slider.value = health / maxHealth;
        if(health <= 0 )
        {
            playerAnim.SetBool("isDead", true);
        }
    }

    public void CameraLogic()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float time = Time.deltaTime;

        rotY += mouseY * time * camRotSpeed;
        rotX = mouseX * time * camRotSpeed;

        if(health > 0)
            playerTr.Rotate(0, rotX, 0);

        rotY = Mathf.Clamp(rotY, minAngle, maxAngle);

        Quaternion localRotation = Quaternion.Euler(-rotY, 0, 0);
        cameraAxis.localRotation = localRotation;

        if(health > 0)
        {
            cameraC.position = Vector3.Lerp(cameraC.position, cameraTrack.position, cameraSpeed * time);
        }
        else
        {
            Vector3 distantPosition = cameraTrack.position + new Vector3(0, 0, -2);
            cameraC.position = Vector3.Lerp(cameraC.position, distantPosition, cameraSpeed * time);
        }
        
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
            if (Input.GetKeyDown(KeyCode.F)){
                foot.isTrigger = true;
                playerAnim.Play("Kick");
                StartCoroutine(DisableColliderAfterAnimation());
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                switch (Random.Range(1, 4))
                {
                    case 1:
                        leftHand.isTrigger = true;
                        playerAnim.Play("Left Punch");
                        StartCoroutine(DisableColliderAfterAnimation());
                        break;
                    case 2:
                        rightHand.isTrigger = true;
                        playerAnim.Play("Right Punch");
                        StartCoroutine(DisableColliderAfterAnimation());
                        break;
                    case 3:
                        rightElbow.isTrigger = true;
                        playerAnim.Play("Elbow Punch");
                        StartCoroutine(DisableColliderAfterAnimation());
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

    IEnumerator DisableColliderAfterAnimation()
    {
        yield return new WaitForSeconds(playerAnim.GetCurrentAnimatorStateInfo(0).length - 2.5f);

        foot.isTrigger = false;
        rightHand.isTrigger = false;
        leftHand.isTrigger = false;
        rightElbow.isTrigger = false;
    }
}
