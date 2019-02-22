using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFire : MonoBehaviour {

    private Animator GunFireAnim;

    public void weaponTriggered()
    {
        GunFireAnim.SetBool("IsTriggered", true);
    }

    // Use this for initialization
    void Start () {
        GunFireAnim = this.GetComponent<Animator>();
    }
	
}
