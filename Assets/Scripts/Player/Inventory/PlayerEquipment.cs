using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [Tooltip("0 = PrimaryWeapon | 1 = SecondaryWeapon | 2 = Armor | 3 = Helmet")]
    public GameObject[] equipment = new GameObject[4];

    [SerializeField] GameObject weaponSwayAndBob;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3) && equipment[0])
        {
            weaponSwayAndBob.SetActive(!equipment[0].activeSelf);
            equipment[0].SetActive(!equipment[0].activeSelf);
            if (equipment[1]) equipment[1].SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && equipment[1])
        {
            weaponSwayAndBob.SetActive(!equipment[1].activeSelf);
            equipment[1].SetActive(!equipment[1].activeSelf);
            if (equipment[0]) equipment[0].SetActive(false);
        }
    }
}
