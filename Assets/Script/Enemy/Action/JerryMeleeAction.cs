using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JerryMeleeAction : NetworkBehaviour
{
    public float AttackDis = 0.5f; //攻击范围

    public void Action(Transform target)
    {
        if (target == null) return;
        if ((transform.position - target.position).sqrMagnitude <= AttackDis)
            Damage(target, 2);
    }

    void Damage(Transform trans, int damageHP)
    {
        if (trans.tag == "Player")
            if (trans.name == "head")
            {
                var playerbody = trans.parent;
                playerbody.GetComponent<PlayerStatusControl>().damagePlayer(3 * damageHP);
            }
            else
            {
                trans.GetComponent<PlayerStatusControl>().damagePlayer(damageHP);
            }
    }
}
