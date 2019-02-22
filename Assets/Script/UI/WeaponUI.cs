using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WeaponUI : MonoBehaviour {

    private GameObject player;
    private Sprite WeaponSprite;
    private bool ImageSet;
    // Use this for initialization
    void Start () {
        player = transform.parent.GetComponent<GameUI>().getCurrentPlayer();
        ImageSet = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (player == null)
        {
            player = transform.parent.GetComponent<GameUI>().getCurrentPlayer();
        }

        else if (player.GetComponent<PlayerEquipmentControl>().isEquip())
        {
            WeaponSprite = player.GetComponent<PlayerEquipmentControl>().Equipment.GetComponent<SpriteRenderer>().sprite;
            this.GetComponent<SpriteRenderer>().sprite = WeaponSprite;
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(WeaponSprite.bounds.size.x, WeaponSprite.bounds.size.y);
        }
        else
        {
            this.GetComponent<SpriteRenderer>().sprite = null;
        }

	}
}
