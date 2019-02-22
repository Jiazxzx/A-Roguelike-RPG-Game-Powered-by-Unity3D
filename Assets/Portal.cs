using UnityEngine;
using UnityEngine.Networking;

public class Portal : NetworkBehaviour
{

    public GameObject target;
    public bool inDoor = false;

    private void Start()
    {
        if(target == null)
        {
            if (tag == "Start")
                target = GameObject.FindWithTag("End");
            else if (tag == "End")
                target = GameObject.FindWithTag("Start");
            else if (tag == "Start2")
                target = GameObject.FindWithTag("End2");
            else if (tag == "End2")
                target = GameObject.FindWithTag("Start2");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (target == null)
        {
            if (tag == "Start")
                target = GameObject.FindWithTag("End");
            else if (tag == "End")
                target = GameObject.FindWithTag("Start");
            else if (tag == "Start2")
                target = GameObject.FindWithTag("End2");
            else if (tag == "End2")
                target = GameObject.FindWithTag("Start2");
        }
        if (!inDoor)
        {
            if (other.tag == "Player")
            {
                target.GetComponent<Portal>().inDoor = true;
                // 可以更改为传送至目标传送门周围某个圆形范围内
                Vector3 randDisperate = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                other.gameObject.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, 0) + randDisperate;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            inDoor = false;
        }
    }
}
