using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerEquipment : MonoBehaviour
{
    Player player;

    [Tooltip("0 = PrimaryWeapon | 1 = SecondaryWeapon | 2 = Armor | 3 = Helmet")]
    public GameObject[] equipment = new GameObject[4];
    public GameObject playerArms;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] TwoBoneIKConstraint rightHandIK;
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    [SerializeField] GameObject weaponMovementsObject;
    WeaponMovements weaponMovements;
    public Weapon weaponHeld;
    Transform muzzleFlash;
    [HideInInspector] public bool isSwitching = false;
    public bool canShoot = true;

    void Start()
    {
        player = GameManager.Instance.player;
        weaponMovements = weaponMovementsObject.GetComponent<WeaponMovements>();
    }

    void Update()
    {
        if (!player.inventoryOpen && !isSwitching)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3) && equipment[0])
            {
                StartCoroutine(SwitchWeapon(equipment[0]));
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && equipment[1])
            {
                StartCoroutine(SwitchWeapon(equipment[1]));
            }

            if (Input.GetMouseButton(0) && weaponHeld && !player.isRunning && canShoot)
            {
                StartCoroutine(Shoot(weaponHeld.rateOfFire));
            }
        }
    }

    IEnumerator SwitchWeapon(GameObject weapon)
    {
        isSwitching = true;

        yield return new WaitForEndOfFrame();

        Animation weaponAnim = weapon.GetComponent<Animation>();

        if (weaponHeld == null)
        {
            weapon.SetActive(true);
            weaponMovementsObject.SetActive(true);
            playerArms.SetActive(true);
            weaponHeld = weapon.GetComponent<Weapon>();

            BuildHandsRig(weapon.transform);

            muzzleFlash = weapon.transform.GetChild(0).Find("MuzzleFlash");

            weaponAnim.Play("WeaponPullOut");
            yield return new WaitForSeconds(weaponAnim.GetClip("WeaponPullOut").length);
        }
        else
        {
            Animation weaponHeldAnim = weaponHeld.GetComponent<Animation>();

            weaponHeldAnim.Play("WeaponPullIn");
            yield return new WaitForSeconds(weaponHeldAnim.GetClip("WeaponPullIn").length);

            weaponHeld.gameObject.SetActive(false);
            weaponMovementsObject.SetActive(false);
            playerArms.SetActive(false);

            if (weaponHeld == weapon.GetComponent<Weapon>())
            {
                weaponHeld = null;
                muzzleFlash = null;
            }
            else
            {
                weapon.SetActive(true);
                weaponMovementsObject.SetActive(true);
                playerArms.SetActive(true);
                weaponHeld = weapon.GetComponent<Weapon>();

                BuildHandsRig(weapon.transform);

                muzzleFlash = weapon.transform.GetChild(0).Find("MuzzleFlash");

                weaponAnim.Play("WeaponPullOut");
                yield return new WaitForSeconds(weaponAnim.GetClip("WeaponPullOut").length);
            }
        }

        isSwitching = false;
    }

    IEnumerator Shoot(float rateOfFire)
    {
        canShoot = false;
        weaponMovements.Recoil(weaponHeld.upRecoil, weaponHeld.sideRecoil, weaponHeld.kickBack);
        yield return new WaitForSeconds(rateOfFire);
        canShoot = true;
    }

    void BuildHandsRig(Transform weapon)
    {
        Transform meshTransform = weapon.GetChild(0);
        Transform rightGrip = meshTransform.Find("RightGrip");
        Transform leftGrip = meshTransform.Find("LeftGrip");
        rightHandIK.data.target = rightGrip;
        leftHandIK.data.target = leftGrip;
        rigBuilder.Build();
    }
}
