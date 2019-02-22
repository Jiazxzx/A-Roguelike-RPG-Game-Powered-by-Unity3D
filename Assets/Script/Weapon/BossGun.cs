using UnityEngine;
using UnityEngine.Networking;

public class BossGun : WeaponControl
{
    public GameObject m_bullet;             //子弹对象
    public float dispareRange;              //子弹散射范围
    public GameObject gunFireMotion;
    private Quaternion m_randomRotation;    //子弹散射控制

    //控制枪的攻击方式
    [Command]
    public void CmdFire(Vector3 bulletPos, Quaternion bulletRot, int damage)
    {
        var bullet = Instantiate(m_bullet, bulletPos, bulletRot) as GameObject;
        bullet.GetComponent<Rigidbody2D>().velocity = -bullet.transform.right * Bullet.m_speed;
        bullet.GetComponent<JerryBullet>().setBulletDamage(damage);
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
        //Boss 向周围8个方向发射子弹 第一种攻击类型

        //随机角度散射
        for (int i = 0; i <= 8; i++)
        {
            m_randomRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + i * 40));
            //创建子弹实例
            Vector3 bulletPos = m_randomRotation * new Vector3(-0.9f, 0, 0) + transform.position;
            CmdFire(bulletPos, m_randomRotation, damage);
        }
        //创建枪口火花实例
        Vector3 FirePos = transform.rotation * new Vector3(-0.5f, 0, 0) + transform.position;
        CmdApplyGunfire(FirePos);

    }

    public override void OnChangeState(bool state)
    {
        Debug.Log("Do nothing");
    }
}
