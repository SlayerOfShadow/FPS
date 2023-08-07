using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerEquipment : MonoBehaviour
{
    [Tooltip("0 = PrimaryWeapon | 1 = SecondaryWeapon | 2 = Armor | 3 = Helmet")]
    public GameObject[] equipment = new GameObject[4];

    [SerializeField] GameObject playerArms;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] TwoBoneIKConstraint rightHandIK;
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    [SerializeField] GameObject weaponSwayAndBob;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3) && equipment[0])
        {
            BuildHandsRig(equipment[0]);
            PullOutWeapon(equipment[0]);
            if (equipment[1]) equipment[1].SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && equipment[1])
        {
            BuildHandsRig(equipment[1]);
            PullOutWeapon(equipment[1]);
            if (equipment[0]) equipment[0].SetActive(false);
        }
    }

    void PullOutWeapon(GameObject weapon)
    {
        playerArms.SetActive(!weapon.activeSelf);
        weaponSwayAndBob.SetActive(!weapon.activeSelf);
        weapon.SetActive(!weapon.activeSelf);
    }

    void BuildHandsRig(GameObject weapon)
    {
        Transform rightGrip = weapon.transform.Find("RightGrip");
        Transform leftGrip = weapon.transform.Find("LeftGrip");
        rightHandIK.data.target = rightGrip;
        leftHandIK.data.target = leftGrip;
        rigBuilder.Build();
    }
}
