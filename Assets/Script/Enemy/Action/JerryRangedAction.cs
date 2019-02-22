using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JerryRangedAction : NetworkBehaviour
{
    public Sprite JerryRanged_left;
    public Sprite JerryRanged_right;

    [SyncVar(hook = "OnChangeDir")]
    public Vector3 WeaponDir;
    public float ShotTime;
    public GameObject m_JerryGun;                    //武器对象
    private GameObject JerryGun;

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
            this.GetComponent<SpriteRenderer>().sprite = JerryRanged_left;
        if (WeaponDir.x >= 0)
            this.GetComponent<SpriteRenderer>().sprite = JerryRanged_right;
    }

    private void Start()
    {
        m_JerryGun.GetComponent<WeaponControl>().player = this.gameObject;
        JerryGun = Instantiate(m_JerryGun, transform.position, transform.rotation) as GameObject;
        JerryGun.transform.SetParent(this.transform);
        //JerryGun.GetComponent<Rigidbody2D>();

        NetworkServer.Spawn(JerryGun);
    }


    public void Action(Transform target)
    {
        if (JerryGun == null) return;

        if(target==null)
        {
            JerryGun.GetComponent<WeaponControl>().UnTrigger();
            Debug.Log("怪兽找不到目标不开枪");
            return;
        }

        if((transform.position - target.position).sqrMagnitude <= FireDis)
        {
            CmdChangeWeaponDir(target.position);

            Debug.Log("Weapon Trigger");
            JerryGun.GetComponent<WeaponControl>().Trigger();
            Debug.Log("怪兽开枪");
        }
        else
        {
            JerryGun.GetComponent<WeaponControl>().UnTrigger();
            Debug.Log("怪兽不开枪");
        }

    }

    [Command]
    private void CmdChangeWeaponDir(Vector3 targetPos)
    {
        //武器随鼠标旋转时朝向调整
        if (JerryGun.transform.eulerAngles.z <= 270 && JerryGun.transform.eulerAngles.z >= 90)
        {
            JerryGun.transform.localScale = new Vector3(1f, -1f);
            adjustWeaponToward = -1f;
        }
        else
        {
            JerryGun.transform.localScale = new Vector3(1f, 1f);
            adjustWeaponToward = 1f;
        }

        //得到从武器到目标的向量
        WeaponDir = targetPos - transform.position;
        //计算角度（tan = y/x）
        angle = Mathf.Atan2(WeaponDir.y, WeaponDir.x) * Mathf.Rad2Deg + 180;
        weaponRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //根据角度进行旋转
        JerryGun.transform.rotation = Quaternion.Slerp(JerryGun.transform.rotation, weaponRotation, JerryGun.GetComponent<WeaponControl>().RotationSpeed * Time.deltaTime);
    }
}
