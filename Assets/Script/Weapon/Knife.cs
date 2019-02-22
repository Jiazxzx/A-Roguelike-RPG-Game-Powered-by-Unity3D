using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Knife : WeaponControl
{
    public GameObject m_knifeHit;
    public GameObject gunFireMotion;


    [Command]
    public void CmdSlash(Vector3 hitPos, int damage)
    {

        var weaponRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 225));

        var knifeHit = Instantiate(m_knifeHit, hitPos, weaponRotation) as GameObject;
        knifeHit.transform.SetParent(this.transform);
        knifeHit.GetComponent<Rigidbody2D>();
        knifeHit.GetComponent<KnifeHit>().setKnifeHitDamage(damage);
        NetworkServer.Spawn(knifeHit);
    }

    [Command]
    public void CmdApplyGunfire(Vector3 firePos)
    {
        var gunFire = Instantiate(gunFireMotion, firePos, transform.rotation) as GameObject;
        NetworkServer.Spawn(gunFire);
    }

    public override void WeaponTriggered(int damage)
    {
        Vector3 knifeHitPos = Quaternion.Euler(transform.eulerAngles) * new Vector3(-0.9f, 0, 0) + transform.position;
        CmdSlash(knifeHitPos, damage);
        Vector3 SlashPos = transform.rotation * new Vector3(-0.5f, 0, 0) + transform.position;
        CmdApplyGunfire(SlashPos);
    }

    public override void OnChangeState(bool state)
    {
        Debug.Log("Do nothing");
    }
}
