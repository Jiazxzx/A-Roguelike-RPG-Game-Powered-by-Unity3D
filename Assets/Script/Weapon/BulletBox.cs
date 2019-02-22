using UnityEngine;
using UnityEngine.Networking;

public class BulletBox : Item
{
    public Sprite bulletBox_close;
    public Sprite bulletBox_open;

    public int ammoAmount = 100;
    GameObject hitObject;
    // Use this for initialization
    void Start ()
    {
        state = false;
	}

    public override void interact(GameObject player)
    {
        if (player.tag != "Player")
            return;
        hitObject = player;
        if (state == false)
            state = true;
    }

    public override void OnChangeState(bool state)
    {
        if (state == false)
            this.GetComponent<SpriteRenderer>().sprite = bulletBox_close;
        if (state == true)
        {
            this.GetComponent<SpriteRenderer>().sprite = bulletBox_open;
            if(isServer)
            {
                hitObject.GetComponent<PlayerStatusControl>().addAmmo(ammoAmount);
            }
        }
    }
}
