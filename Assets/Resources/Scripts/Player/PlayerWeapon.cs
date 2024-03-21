using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerWeapon : NetworkBehaviour
{
    private LayerMask layerMask;
    private int gunDamage;
    private int bulletsPerMag;
    private int bulletsInReserves;
    private float fireRate;
    private float weaponRange;

    private float baseInaccuracyScope;
    private float baseInaccuracyHip;
    private float inaccuracyIncreaseOverTime;
    private float inaccuracyDecreaseOverTime;
    private float maximumInaccuracy;
    private float maxInaccuracyHip;
    private float maxInaccuracyScope;
    private float triggerTime = 0.05f;
    private float baseInaccuracy;

    private bool scoped;

    private int pelletsPerShot = 10;

    public MouseLook mouseLook;


    [Header("Crosshair")]

    [Header("Spray")]
    public float coneLength = 10f;

    [Header("Bulletmarks")]
    public GameObject defaultBullet;

    [Header("Audio")]
    public AudioSource aSource;
    private AudioClip soundFire;
    private AudioClip soundReload;

    [Header("Other")]
    private FpsController controller;
    public Text ammoText;
    public Camera weaponCam;
    public GameObject muzzleFlash;

    private PlayerMovement movement;
    private WeaponProperties properties;
    private WeaponManager weaponManager;

    [HideInInspector]
    public bool reloading = false;
    [HideInInspector]
    public bool selected = false;
    private bool isFiring = false;
    private int lastFrameShot = -10;
    private float nextFire;
    private int bulletsInMag;
    private RaycastHit hit;
    private bool canShoot;

    private PlayerInput input;

    void Start()
    {
        controller = GetComponent<FpsController>();
        movement = GetComponent<PlayerMovement>();
        input = GetComponent<PlayerInput>();
        weaponManager = GetComponent<WeaponManager>();

        if (isLocalPlayer)
            canShoot = true;
    }

    void Update()
    {
        if (input.currentInput.inputFireTap)
        {
            if (properties.mode == WeaponProperties.fireMode.single)
            {
                FireSemi();
            }

            if (bulletsInMag > 0)
            {
                isFiring = true;
            }
        }

        if (input.currentInput.inputFireHold)
        {
            if (properties.mode == WeaponProperties.fireMode.auto)
            {
                FireSemi();
                if (bulletsInMag > 0)
                {
                    isFiring = true;
                }
            }
        }

        if (input.currentInput.inputReload)
        {
            StartCoroutine(Reload());
        }

        if (scoped)
        {
            maximumInaccuracy = maxInaccuracyScope;
            baseInaccuracy = baseInaccuracyScope;
        }
        else
        {
            maximumInaccuracy = maxInaccuracyHip;
            baseInaccuracy = baseInaccuracyHip;
        }

        if (movement.moveDirection.magnitude > 3.0)
        {
            triggerTime += inaccuracyDecreaseOverTime;
        }

        if (isFiring)
        {
            triggerTime += inaccuracyIncreaseOverTime;
        }
        else
        {
            if (movement.moveDirection.magnitude < 3.0)
                triggerTime -= inaccuracyDecreaseOverTime;
        }

        if (triggerTime >= maximumInaccuracy)
            triggerTime = maximumInaccuracy;

        if (triggerTime <= baseInaccuracy)
            triggerTime = baseInaccuracy;

        if (nextFire > Time.time)
            isFiring = false;


    }

    void LateUpdate()
    {
        ammoText.text = (bulletsInMag + " / " + bulletsInReserves);

        if (lastFrameShot == Time.frameCount)
        {
            for (int i = 0; i < weaponManager.weaponsInUse.Length; i++)
            {
                if (weaponManager.weaponsInUse[i].activeSelf)
                {
                    foreach (Transform t in weaponManager.weaponsInUse[i].transform)
                    {
                        if (t.name == "FlashPoint")
                        {
                            Instantiate(muzzleFlash, t.position, t.rotation, t);
                        }
                    }
                }
            }
        }
    }

    void FireSemi()
    {
        if (reloading || bulletsInMag <= 0)
        {
            return;
        }

        if (Time.time - fireRate > nextFire)
            nextFire = Time.time - Time.deltaTime;

        while (nextFire < Time.time)
        {
            if (nextFire > Time.time)
            {
                if (bulletsInMag <= 0)
                {
                    // Call method to handle being out of ammo and trying to shoot
                }
            }
            else
            {
                // Conical Innacuracy
                float randomRadius = Random.Range(0, triggerTime);
                float randomAngle = Random.Range(0, 2 * Mathf.PI);

                //Calculating the raycast direction
                Vector3 direction = new Vector3(randomRadius * Mathf.Cos(randomAngle), randomRadius * Mathf.Sin(randomAngle), coneLength);

                direction = mouseLook.transform.TransformDirection(direction.normalized);

                FireOneBullet(direction, weaponCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)));
                aSource.clip = soundFire;
                aSource.PlayOneShot(aSource.clip);
                lastFrameShot = Time.frameCount;

                bulletsInMag--;
            }

            nextFire = Time.time + fireRate;
        }

        if (isLocalPlayer && !isServer)
            CmdFireSemi();

        if (isLocalPlayer && isServer)
            RpcFireSemi();
    }

    [Command]
    void CmdFireSemi()
    {
        FireSemi();
        RpcFireSemi();
    }

    [ClientRpc]
    void RpcFireSemi()
    {
        if (!isLocalPlayer)
            FireSemi();
    }

    void FireShotgun()
    {

    }

    void FireOneBullet(Vector3 dir, Vector3 pos)
    {
        if (Physics.Raycast(pos, dir, out hit, weaponRange, layerMask))
        {
            Vector3 contact = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            float randomScale = Random.Range(0.2f, 0.6f);

            if (hit.collider.tag == "Damage")
            {
                hit.collider.SendMessage("TakeDamage", gunDamage, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                GameObject bulletMark = Instantiate(defaultBullet, contact, rot) as GameObject;
                bulletMark.transform.localPosition += 0.02f * hit.normal;
                bulletMark.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                bulletMark.transform.parent = hit.transform;
            }
        }
    }

    void FirePellet()
    {

    }

    IEnumerator Reload()
    {
        if (reloading)
            yield break;

        if (bulletsInMag != bulletsPerMag)
        {
            reloading = true;
            aSource.clip = soundReload;
            aSource.PlayOneShot(soundReload, 0.7f);
            yield return new WaitForSeconds(0.6f);
            bulletsInMag = bulletsPerMag;
            reloading = false;
            yield break;
        }

        /* Code for ammo based of reserves
        if (bulletsInReserves > 0 && bulletsInMag != bulletsPerMag)
        {
            if (bulletsInReserves + bulletsInMag >= bulletsPerMag)
            {
                reloading = true;
                //weaponAnim shit
                aSource.clip = soundReload;
                aSource.PlayOneShot(soundReload, 0.7f);
                // yield return new WaitForSeconds(reloadTime);
                bulletsInReserves -= bulletsPerMag - bulletsInMag;
                bulletsInMag = bulletsPerMag;
                reloading = false;
                yield break;
            }
            else
            {
                reloading = true;
                //weaponAnim shit
                aSource.clip = soundReload;
                aSource.PlayOneShot(soundReload, 0.7f);
                // yield return new WaitForSeconds(reloadTime);
                bulletsInMag += bulletsInReserves;
                bulletsInReserves = 0;
                reloading = false;
                yield break;
            }
        }
        */
    }

    public void UpdateWeaponProperties(GameObject SelectedWeapon)
    {
        properties = SelectedWeapon.GetComponent<WeaponProperties>();

        bulletsInMag = properties.bulletsPerMag;
        bulletsInReserves = properties.bulletsInReserves;

        layerMask = properties.layerMask;
        gunDamage = properties.gunDamage;
        bulletsPerMag = properties.bulletsPerMag;
        fireRate = properties.fireRate;
        weaponRange = properties.weaponRange;

        baseInaccuracyScope = properties.baseInaccuracyScope;
        baseInaccuracyHip = properties.baseInaccuracyHip;
        inaccuracyIncreaseOverTime = properties.inaccuracyIncreaseOverTime;
        inaccuracyDecreaseOverTime = properties.inaccuracyDecreaseOverTime;
        maxInaccuracyHip = properties.maxInaccuracyHip;
        maxInaccuracyScope = properties.maxInaccuracyScope;

        pelletsPerShot = properties.pelletsPerShot;

        soundFire = properties.soundFire;
        soundReload = properties.soundReload;
    }
}
