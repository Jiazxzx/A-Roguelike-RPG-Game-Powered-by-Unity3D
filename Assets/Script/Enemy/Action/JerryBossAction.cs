using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JerryBossAction : NetworkBehaviour
{
    public Sprite JerryBoss_left;
    public Sprite JerryBoss_right;

    [SyncVar(hook = "OnChangeDir")]
    public Vector3 WeaponDir;
    public float ShotTime;
    public GameObject m_BossGun;                    //武器对象
    private GameObject BossGun;

    public float FireDis = 15;

    private Quaternion weaponRotation;              //武器旋转方向
    [SyncVar]
    int sortingOrderOfWeapon = 0;
    [SyncVar]
    float adjustWeaponToward = 1f;
    [SyncVar]
    float angle = 0;

    void OnChangeDir(Vector3 WeaponDir)
    {
        if (WeaponDir.x < 0)
            this.GetComponent<SpriteRenderer>().sprite = JerryBoss_left;
        if (WeaponDir.x >= 0)
            this.GetComponent<SpriteRenderer>().sprite = JerryBoss_right;
    }

    private void Start()
    {
        m_BossGun.GetComponent<WeaponControl>().player = this.gameObject;
        BossGun = Instantiate(m_BossGun, transform.position, transform.rotation) as GameObject;
        BossGun.transform.SetParent(this.transform);
        //BossGun.GetComponent<Rigidbody2D>();

        NetworkServer.Spawn(BossGun);
    }


    public void Action(Transform target)
    {
        if (BossGun == null) return;

        if (target == null)
        {
            BossGun.GetComponent<WeaponControl>().UnTrigger();
            Debug.Log("怪兽找不到目标不开枪");
            return;
        }

        if ((transform.position - target.position).sqrMagnitude <= FireDis)
        {
            CmdChangeWeaponDir(target.position);

            Debug.Log("Weapon Trigger");
            BossGun.GetComponent<WeaponControl>().Trigger();
            Debug.Log("怪兽开枪");
        }
        else
        {
            BossGun.GetComponent<WeaponControl>().UnTrigger();
            Debug.Log("怪兽不开枪");
        }

    }

    [Command]
    private void CmdChangeWeaponDir(Vector3 targetPos)
    {
        //武器随鼠标旋转时朝向调整
        if (BossGun.transform.eulerAngles.z <= 270 && BossGun.transform.eulerAngles.z >= 90)
        {
            BossGun.transform.localScale = new Vector3(1f, -1f);
            adjustWeaponToward = -1f;
        }
        else
        {
            BossGun.transform.localScale = new Vector3(1f, 1f);
            adjustWeaponToward = 1f;
        }

        //得到从武器到目标的向量
        WeaponDir = targetPos - transform.position;
        //计算角度（tan = y/x）
        angle = Mathf.Atan2(WeaponDir.y, WeaponDir.x) * Mathf.Rad2Deg + 180;
        weaponRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //根据角度进行旋转
        BossGun.transform.rotation = Quaternion.Slerp(BossGun.transform.rotation, weaponRotation, BossGun.GetComponent<WeaponControl>().RotationSpeed * Time.deltaTime);
    }
}
