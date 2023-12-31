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
    public GameObject weaponMovementsObject;
    WeaponMovements weaponMovements;
    public Weapon weaponHeld;
    [HideInInspector] public Transform muzzleFlash;
    [SerializeField] GameObject muzzleFlashEffect;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public bool isSwitching = false;
    bool canShoot = true;
    [SerializeField] GameObject bulletImpact;
    [SerializeField] GameObject bulletHole;

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
            audioSource = weapon.GetComponent<AudioSource>();

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
                audioSource = null;
            }
            else
            {
                weapon.SetActive(true);
                weaponMovementsObject.SetActive(true);
                playerArms.SetActive(true);
                weaponHeld = weapon.GetComponent<Weapon>();

                BuildHandsRig(weapon.transform);

                muzzleFlash = weapon.transform.GetChild(0).Find("MuzzleFlash");
                audioSource = weapon.GetComponent<AudioSource>();

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
        audioSource.PlayOneShot(audioSource.clip);
        Instantiate(muzzleFlashEffect, muzzleFlash);

        RaycastHit hit;
        if (Physics.Raycast(player.playerCamera.transform.position, player.playerCamera.transform.forward, out hit, weaponHeld.range))
        {
            Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
            Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
        }

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
