using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform shootSpawn;

    public bool shooting = false;

    public int weaponType;

    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(shootSpawn.position, shootSpawn.forward * 10f, Color.red);
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward * 10f, Color.blue);

        RaycastHit cameraHit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out cameraHit))
        {
            Vector3 shootDirection = cameraHit.point - shootSpawn.position;
            shootSpawn.rotation = Quaternion.LookRotation(shootDirection);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Shoot();
            }
        }
    }

    public void Shoot()
    {
        Vector3 realShootPosition = new Vector3();

        if (weaponType == 1)
        {
            realShootPosition = new Vector3(shootSpawn.position.x, -0.10f, shootSpawn.position.z);
        }
        if (weaponType == 2)
        {
            realShootPosition = new Vector3(shootSpawn.position.x, shootSpawn.position.y , shootSpawn.position.z);
        }

        Instantiate(bulletPrefab, realShootPosition, shootSpawn.rotation);
    }
}
