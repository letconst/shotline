using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : SingletonMonoBehaviour<WeaponManager>
{
    class WeaponBase
    {
        public string Name { get; set; }
    }

    class Weapon1 : WeaponBase
    {
        public float BulletSpeed_1 { get; set; }
        public float LineGageMax_1 { get; set; }
        public float LineGageRecovery_1 { get; set; }
    }

    class Weapon2 :WeaponBase
    {
        public float BulletSpeed_2 { get; set; }
        public float LineGageMax_2 { get; set; }
        public float LineGageRecovery_2 { get; set; }
    }

    class Weapon3 :WeaponBase
    {
        public float BulletSpeed_3 { get; set; }
        public float LineGageMax_3 { get; set; }
        public float LineGageRecovery_3 { get; set; }
    }
}
