using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[System.Runtime.InteropServices.Guid("78A7C5BE-FD9C-460E-B06D-967736406E62")]
public class WeaponManager : NetworkBehaviour
{
    [SyncVar(hook = "ClientDeselectWeapon")] public int weaponToSelect = 0;
    [SyncVar] public int selectWepSlot1 = 0;
    [SyncVar] public int selectWepSlot2 = 0;

    public GameObject[] weaponsInUse;
    public GameObject[] weaponsInGame;
    public GameObject weapons;

    [HideInInspector] public int weaponToDrop;
    public bool canSwitch = true;
    private float switchWeaponTime = 0.02f;
    private PlayerWeapon PlayerWeapon;

    void Start()
    {
        PlayerWeapon = GetComponent<PlayerWeapon>();
        // Load available weapons
        weaponsInGame = new GameObject[GetFirstChildren(weapons.transform).Length];
        weaponsInUse = new GameObject[2];
        for (int i = 0; i < weaponsInGame.Length; i++)
        {
            weaponsInGame[i] = GetFirstChildren(weapons.transform)[i].gameObject;
            weaponsInGame[i].SetActive(false);
        }

        weaponsInUse[0] = weaponsInGame[selectWepSlot1];
        weaponsInUse[1] = weaponsInGame[selectWepSlot2];
        InitiateWeaponSwitch();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Cursor.lockState == CursorLockMode.None)
            return;

        if (Input.GetKeyDown("1") && weaponsInUse.Length >= 1 && canSwitch && weaponToSelect != 0)
        {
            weaponToSelect = 0;
            InitiateWeaponSwitch();

        }
        else if (Input.GetKeyDown("2") && weaponsInUse.Length >= 2 && canSwitch && weaponToSelect != 1)
        {
            weaponToSelect = 1;
            InitiateWeaponSwitch();

        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && canSwitch)
        {
            weaponToSelect++;
            if (weaponToSelect > (weaponsInUse.Length - 1))
            {
                weaponToSelect = 0;
            }
            InitiateWeaponSwitch();
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && canSwitch)
        {
            weaponToSelect--;
            if (weaponToSelect < 0)
            {
                weaponToSelect = weaponsInUse.Length - 1;
            }
            InitiateWeaponSwitch();
        }
    }

    public override void OnStartClient()
    {
        ClientDeselectWeapon(weaponToSelect);
        UpdateWeaponLoadoutOne(selectWepSlot1);
        UpdateWeaponLoadoutTwo(selectWepSlot2);

    }

    private void InitiateWeaponSwitch()
    {
        CmdDeselectWeapon(weaponToSelect);
        StartCoroutine(DeselectWeapon());
    }

    [Command]
    private void CmdDeselectWeapon(int newWeapon)
    {
        weaponToSelect = newWeapon;
        StartCoroutine(DeselectWeapon());
    }

    private void ClientDeselectWeapon(int newWeapon)
    {
        weaponToSelect = newWeapon;
        if (isLocalPlayer)
            return;

        StartCoroutine(DeselectWeapon());
    }

    IEnumerator DeselectWeapon()
    {
        canSwitch = false;

        for (int i = 0; i < weaponsInUse.Length; i++)
        {
            weaponsInUse[i].SendMessage("Deselect", SendMessageOptions.DontRequireReceiver);
            weaponsInUse[i].gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(switchWeaponTime);
        SelectWeapon(weaponToSelect);
        yield return new WaitForSeconds(switchWeaponTime);
        canSwitch = true;
    }

    public void SelectWeapon(int i)
    {
        weaponsInUse[i].gameObject.SetActive(true);
        weaponsInUse[i].SendMessage("DrawWeapon", SendMessageOptions.DontRequireReceiver);
        PlayerWeapon.UpdateWeaponProperties(weaponsInUse[i]);
    }

    public void ChooseWeaponLoadout(int index)
    {
        StartCoroutine(DeselectWeapon());

        if (weaponToSelect == 0)
        {
            selectWepSlot2 = selectWepSlot1;
            selectWepSlot1 = index;
        }
        else if (weaponToSelect == 1)
        {
            selectWepSlot1 = selectWepSlot2;
            selectWepSlot2 = index;
        }

        weaponsInUse[0] = weaponsInGame[selectWepSlot1];
        weaponsInUse[1] = weaponsInGame[selectWepSlot2];

        if (!isServer)
            CmdChooseWeaponLoadout(selectWepSlot1, selectWepSlot2);
        else
            RpcChooseWeaponLoadout(selectWepSlot1, selectWepSlot2);
    }

    [Command]
    private void CmdChooseWeaponLoadout(int slotOne, int slotTwo)
    {
        StartCoroutine(DeselectWeapon());

        selectWepSlot1 = slotOne;
        selectWepSlot2 = slotTwo;
        weaponsInUse[0] = weaponsInGame[selectWepSlot1];
        weaponsInUse[1] = weaponsInGame[selectWepSlot2];

        RpcChooseWeaponLoadout(slotOne, slotTwo);
    }

    [ClientRpc]
    private void RpcChooseWeaponLoadout(int slotOne, int slotTwo)
    {
        if (isLocalPlayer)
            return;

        StartCoroutine(DeselectWeapon());

        selectWepSlot1 = slotOne;
        selectWepSlot2 = slotTwo;

        weaponsInUse[0] = weaponsInGame[selectWepSlot1];
        weaponsInUse[1] = weaponsInGame[selectWepSlot2];
    }

    private void UpdateWeaponLoadoutOne(int slotOne)
    {
        selectWepSlot1 = slotOne;
    }

    private void UpdateWeaponLoadoutTwo(int slotTwo)
    {
        selectWepSlot2 = slotTwo;
    }

    Transform[] GetFirstChildren(Transform parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true);
        Transform[] firstChildren = new Transform[parent.childCount];
        int index = 0;
        foreach (Transform child in children)
        {
            if (child.parent == parent)
            {
                firstChildren[index] = child;
                index++;
            }
        }
        return firstChildren;
    }

    public void LMG()
    {
        ChooseWeaponLoadout(2);
    }

    public void Rifle()
    {
        ChooseWeaponLoadout(3);
    }

    public void Secondary()
    {
        ChooseWeaponLoadout(1);
    }

    public void Shotgun()
    {
        ChooseWeaponLoadout(5);
    }

    public void Sniper()
    {
        ChooseWeaponLoadout(4);
    }
}
