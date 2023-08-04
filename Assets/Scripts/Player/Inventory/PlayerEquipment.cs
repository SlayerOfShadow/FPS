using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [Tooltip("0 = PrimaryWeapon | 1 = SecondaryWeapon | 2 = Armor | 3 = Helmet")]
    public GameObject[] equipment = new GameObject[4];
}
