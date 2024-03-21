using UnityEngine;

public class WeaponProperties : MonoBehaviour
{

    // These script only lists the properties of the weapon it is attached to

    public enum fireMode { none, single, semi, auto, burst, shotgun };
    public enum Aim { Simple, Sniper };

    public fireMode mode = fireMode.single;
    public Aim aimMode = Aim.Simple;

    [Header("Weapon configuration")]
    public LayerMask layerMask;
    public int gunDamage = 1;
    public int bulletsPerMag = 10;
    public int magazines = 3;
    public float fireRate = 0.05f;
    public float weaponRange = 1000f;
    public int bulletsInMag;
    public int bulletsInReserves;

    [Header("Accuracy Settings")]
    public float baseInaccuracyScope = 0.005f;
    public float baseInaccuracyHip = 1.5f;
    public float inaccuracyIncreaseOverTime = 0.2f;
    public float inaccuracyDecreaseOverTime = 0.5f;
    public float maxInaccuracyHip = 5.0f;
    public float maxInaccuracyScope = 1.0f;

    [Header("Aiming")]

    [Header("Shotgun Settings")]
    public int pelletsPerShot = 10;

    [Header("Audio")]
    public AudioClip soundFire;
    public AudioClip soundReload;

    private int lastFrameShot = -10;

    void Start()
    {
        bulletsInMag = bulletsPerMag;
        bulletsInReserves = bulletsPerMag * magazines;
    }
}
