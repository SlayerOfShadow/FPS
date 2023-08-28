using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public Vector3 weaponBasePosition;
    public Quaternion weaponBaseRotation;
    public Vector3 weaponAimPosition;
    public Quaternion weaponAimRotation;
    public float rateOfFire;
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float kickBackZ;
}
