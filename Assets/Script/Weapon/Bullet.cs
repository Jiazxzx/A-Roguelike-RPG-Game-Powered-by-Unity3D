using UnityEngine;

public class Bullet : MonoBehaviour {

    static public float m_speed = 10f;         //飞行速度
    public float m_liveTime = 1;        //持续时间
    public GameObject exp;
    public GameObject strike;

    public int BulletDamage;

    protected Transform m_transform;

    public void setBulletDamage(int d)
    {
        BulletDamage = d;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hit = collision.gameObject;
        if (hit.tag == "monster")
        {
            hit.GetComponent<EnemyStatusControl>().damageEnmey(BulletDamage);
        }
        if(hit.tag == "Item")
        {
            hit.GetComponent<Item>().interact(this.gameObject);
        }
        if (hit.tag != "Detector" && hit.tag != "skip" && hit.tag != "Player")
        {
            Instantiate(exp, m_transform.position, m_transform.rotation);
            Destroy(gameObject);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)//被打
    {
        if (collision.tag == "Player")
            if (collision.name == "head")
            {
                var playerbody = collision.transform.parent;
                Debug.Log("Strike");
                playerbody.GetComponent<PlayerStatusControl>().damagePlayer(3 * BulletDamage);
                Instantiate(strike, m_transform.position, m_transform.rotation);
                Destroy(gameObject);
            }
            else
            {
                collision.GetComponent<PlayerStatusControl>().damagePlayer(BulletDamage);
                Instantiate(exp, m_transform.position, m_transform.rotation);
                Destroy(gameObject);
            }

    }

    public void OnDestroy()
    {
        //Instantiate(exp,m_transform.position,m_transform.rotation);
    }

    // Use this for initialization
    void Start ()
    {
        m_transform = this.transform;
        Destroy(this.gameObject, m_liveTime);
    }
}
