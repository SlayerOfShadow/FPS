using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerEquipment : MonoBehaviour
{
    [Tooltip("0 = PrimaryWeapon | 1 = SecondaryWeapon | 2 = Armor | 3 = Helmet")]
    public GameObject[] equipment = new GameObject[4];

    public GameObject playerArms;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] TwoBoneIKConstraint rightHandIK;
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    [SerializeField] GameObject weaponSwayAndBob;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3) && equipment[0])
        {
            if (equipment[1]) equipment[1].SetActive(false);
            BuildHandsRig(equipment[0]);
            StartCoroutine(DelayedPullOut(equipment[0]));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && equipment[1])
        {
            if (equipment[0]) equipment[0].SetActive(false);
            BuildHandsRig(equipment[1]);
            StartCoroutine(DelayedPullOut(equipment[1]));
        }
    }

    void PullOutWeapon(GameObject weapon)
    {
        weaponSwayAndBob.SetActive(!weapon.activeSelf);
        weapon.SetActive(!weapon.activeSelf);
        playerArms.SetActive(weapon.activeSelf);
    }

    void BuildHandsRig(GameObject weapon)
    {
        Transform rightGrip = weapon.transform.Find("RightGrip");
        Transform leftGrip = weapon.transform.Find("LeftGrip");
        rightHandIK.data.target = rightGrip;
        leftHandIK.data.target = leftGrip;
        rigBuilder.Build();
    }

    IEnumerator DelayedPullOut(GameObject weapon)
    {
         yield return new WaitForEndOfFrame();
         PullOutWeapon(weapon);
    }
}
