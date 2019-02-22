using UnityEngine;
using UnityEngine.Networking;

public class MedicalKit : Item
{
    public int healAmount = 100;
    GameObject hitObject;
    // Use this for initialization
    void Start()
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
        if (state == true)
        {
            if (isServer)
            {
                hitObject.GetComponent<PlayerStatusControl>().healPlayer(healAmount);
            }
            Destroy(gameObject);
        }
    }
}
