using UnityEngine;
using UnityEngine.Networking;

public class PlayerInteractionControl : NetworkBehaviour {

    public bool KnockedToInteract = true; //碰撞互动开关

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (KnockedToInteract && (collision.collider.tag == "Item" || collision.collider.tag == "Weapon"))
        {
            collision.collider.GetComponent<Item>().interact(this.gameObject);
        }
    }
}
