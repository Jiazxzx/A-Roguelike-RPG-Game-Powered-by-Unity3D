using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制敌人的HP状态
/// 需要对敌人造成伤害时调用damage(int)方法
/// 每当造成伤害会尝试调用敌人上的ItemOnHit()方法
/// 物体HP耗尽时会尝试调用敌人上的Die()方法
/// </summary>


public class EnemyStatusControl : MonoBehaviour {

    public int MaxHP = 4;
    public int damageAmount = 1;
    public GameObject dieMotion; //jerry死亡动画

    private SpriteRenderer spriteRender;
    private bool isHit; //Jerry是否为击中状态
    private float flashTimer; //击中闪光计时器
    private int HP;
    private bool IsDead = false;

    public int getCurrentHP()
    {
        return HP;
    }

    public int getMaxHP()
    {
        return MaxHP;
    }

    public bool isDead()
    {
        return IsDead;
    }

    public void damageEnmey(int d)
    {
        HP -= d;
        isHit = true;
        BroadcastMessage("SetSlider", true);
        if (HP <= 0)
        {
            IsDead = true;
            EnemyDie();
        }
    }

    public void EnemyDie()
    {
        Destroy(this.gameObject);
        Instantiate(dieMotion, transform.position, transform.rotation);
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            collision.collider.GetComponent<PlayerStatusControl>().damagePlayer(damageAmount);
        }
    }

    // Use this for initialization
    void Start () {
        HP = MaxHP;
        flashTimer = 0.1f;
        spriteRender = GetComponent<SpriteRenderer>();
    }


    private void Update()
    {

        if (isHit)
        {

            flashTimer -= Time.deltaTime;

            if (flashTimer > 0)
            {
                spriteRender.color = new Color(0.9f, 0.5f, 0.5f, 1f);
                Debug.Log("Flash");
            }
            else
            {
                isHit = false;
                flashTimer = 0.1f;
                spriteRender.color = new Color(1f, 1f, 1f, 1f);
            }

        }

    }
}
