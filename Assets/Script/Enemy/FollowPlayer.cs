using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject PlayerGo;//追击的目标
    public float Speed;//移动速度
    public float distance;//跟踪范围
    private Rigidbody2D rb2d;
    // Use this for initialization
    void Start()
    {
        if(PlayerGo == null)
            PlayerGo = GameObject.Find("Player(Clone)");
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos1 = this.transform.position;//怪物位置
        Vector3 pos2 = PlayerGo.transform.position;//玩家位置
        if (Mathf.Sqrt(Mathf.Pow((pos1.x - pos2.x), 2) + Mathf.Pow((pos1.y - pos2.y), 2)) <= distance)
        {
            var dir = (PlayerGo.transform.position - transform.position).normalized;//追击方向
            rb2d.MovePosition(transform.position + dir * Time.deltaTime * Speed);

            BroadcastMessage("SetSlider", true);
        }

    }
}