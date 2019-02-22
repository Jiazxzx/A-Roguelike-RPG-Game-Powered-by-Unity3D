using UnityEngine;
using UnityEngine.Networking;

public class ShotGun : WeaponControl
{

    public GameObject m_bullet;             //子弹对象
    public float dispareRange;              //子弹散射范围
    public GameObject gunFireMotion;
    private Quaternion rotation_front;    //子弹散射控制
    private Quaternion rotation_left;    //子弹散射控制
    private Quaternion rotation_rifht;    //子弹散射控制

    private void Start()
    {
        AmmoNeeded = 3;
        dispareRange = 30f;
        AttackSpeed = 1f;
    }

    //控制枪的攻击方式
    [Command]
    public void CmdFire(Vector3 bulletPos, Quaternion bulletRot, int damage)
    {
        var bullet = Instantiate(m_bullet, bulletPos, bulletRot) as GameObject;
        bullet.GetComponent<Rigidbody2D>().velocity = -bullet.transform.right * Bullet.m_speed;
        bullet.GetComponent<Bullet>().setBulletDamage(damage);
        NetworkServer.Spawn(bullet);
    }

    [Command]
    public void CmdApplyGunfire(Vector3 firePos)
    {
        var gunFire = Instantiate(gunFireMotion, firePos, transform.rotation) as GameObject;
        NetworkServer.Spawn(gunFire);
    }

    public override void WeaponTriggered(int damage)
    {
        //随机角度散射
        rotation_front = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z));
        rotation_left = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 20f));
        rotation_rifht = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 20f));
        //创建子弹实例
        Vector3 bulletPos1 = rotation_front * new Vector3(-0.9f, 0, 0) + transform.position;
        Vector3 bulletPos2 = rotation_left * new Vector3(-0.9f, 0.02f, 0) + transform.position;
        Vector3 bulletPos3 = rotation_rifht * new Vector3(-0.9f, -0.02f, 0) + transform.position;
        CmdFire(bulletPos1, rotation_front, damage);
        CmdFire(bulletPos2, rotation_left, damage);
        CmdFire(bulletPos3, rotation_rifht, damage);
        //创建枪口火花实例
        Vector3 FirePos = transform.rotation * new Vector3(-0.5f, 0, 0) + transform.position;
        CmdApplyGunfire(FirePos);
    }

    public override void OnChangeState(bool state)
    {
        Debug.Log("Do nothing");
    }
}
