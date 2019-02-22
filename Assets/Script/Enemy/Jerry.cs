using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Jerry的个性化定制部分
/// 需要为EnemyStatusControl.cs提供死亡时调用的方法ItemDie()以及被击中调用的方法ItemOnHit()
/// 
/// </summary>

public class Jerry : MonoBehaviour
{

    public GameObject dieMotion; //jerry死亡动画

    protected Transform m_transform;
    private SpriteRenderer spriteRender;
    private bool isHit; //Jerry是否为击中状态
    private float flashTimer; //击中闪光计时器


    protected void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        m_transform = this.transform;
        flashTimer = 0.1f;
    }

    public void ItemDie()
    {
        Destroy(this.gameObject);
        Instantiate(dieMotion, m_transform.position, m_transform.rotation);
    }

    public void ItemOnHit()
    {
        isHit = true;
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
