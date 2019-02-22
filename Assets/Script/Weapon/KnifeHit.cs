using UnityEngine;

public class KnifeHit : MonoBehaviour
{
    public float m_liveTime = 0.1f;        //持续时间
    public GameObject exp;
    public GameObject strike;

    public int knifeDamage;

    protected Transform m_transform;

    public void setKnifeHitDamage(int d)
    {
        knifeDamage = d;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hit = collision.gameObject;
        if (hit.tag == "Item")
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
        GameObject hit = collision.gameObject;
        if (collision.transform.parent == this.transform.parent.parent|| collision.transform == this.transform.parent.parent)
        {
            return;
        }
        if (collision.tag == "Player")
        {
            Debug.Log(collision.name);
            if (collision.name == "head")
            {
                var playerbody = collision.transform.parent;
                Debug.Log("Strike");
                playerbody.GetComponent<PlayerStatusControl>().damagePlayer(3 * knifeDamage);
                Instantiate(strike, m_transform.position, m_transform.rotation);
            }
            else
            {
                collision.GetComponent<PlayerStatusControl>().damagePlayer(knifeDamage);
                Instantiate(exp, m_transform.position, m_transform.rotation);
            }
        }

        if (hit.tag == "monster")
        {
            hit.GetComponent<EnemyStatusControl>().damageEnmey(knifeDamage);
        }

    }

    public void OnDestroy()
    {
        //Instantiate(exp,m_transform.position,m_transform.rotation);
    }

    // Use this for initialization
    void Start()
    {
        m_transform = this.transform;
        Destroy(this.gameObject, m_liveTime);
    }
}
