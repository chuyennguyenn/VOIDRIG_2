using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    public GameObject weapon1;
    public GameObject weapon2;
    public GameObject player;

    private bool isWeapon1Active = true;
    private bool isSwitching = false;

    private float posX;
    private float posY;
    private float posZ;


    private PlayerInput playerInput;
    private InputAction switchWeaponAction;


    private void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        weapon1 = Instantiate(weapon1, transform.position, Quaternion.identity);
        weapon2 = Instantiate(weapon2, transform.position, Quaternion.identity);

        weapon1.transform.parent = player.transform;
        weapon2.transform.parent = player.transform;
        weapon1.transform.position = transform.position;
        weapon2.transform.position = transform.position;

        weapon1.SetActive(false);
        weapon2.SetActive(false);
    }
    private void Start()
    {
       

        switchWeaponAction = playerInput.actions["SwitchWeapon"];
        //Debug.Log("Heloo");

        weapon1.SetActive(true);
        //weapon1.tag = "MachineGun";
        weapon2.SetActive(false);
       
    }

    private void Update()
    {
        if (switchWeaponAction.WasPressedThisFrame())
        {
            Debug.Log("Switching Weapon");
            switchWeapon();
        }

        isSwitching = false;
    }

    private void switchWeapon()
    {
        isSwitching = true;

        if (isSwitching)
        {
            isWeapon1Active = !isWeapon1Active;
        }
       
        if (isWeapon1Active)
        {
            weapon1.SetActive(false);
            weapon2.SetActive(true);
        }
        else
        {
            weapon1.SetActive(true);
            weapon2.SetActive(false);
        }
    }
}
