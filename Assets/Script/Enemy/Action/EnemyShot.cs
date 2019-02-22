using UnityEngine;
using UnityEngine.Networking;

public class EnemyShot : NetworkBehaviour
{
    public enum ENEMY_TOWARDS { UP, DOWN, RIGHT, LEFT };
    public GameObject weaponPrefab;
    public GameObject equipment;
    private Quaternion weaponRotation;              //武器旋转方向

    [SyncVar] public ENEMY_TOWARDS enemyTowards = ENEMY_TOWARDS.DOWN;
    [SyncVar] int sortingOrderOfWeapon = 0;
    [SyncVar] float adjustWeaponToward = 1f;
    [SyncVar] float angle = 0;

    //private void Start()
    //{
    //    enemyTowards = ENEMY_TOWARDS.DOWN;
    //}

    //private void Update()
    //{
    //    if (!isServer)
    //        return;

    //    if(GetComponent<>)
    //}

    public void AttakPlayer(GameObject target)
    {
        if(equipment == null)
        {
            equipment = Instantiate(weaponPrefab, transform.position - new Vector3(0, 0, 10), Quaternion.identity) as GameObject;
            equipment.transform.SetParent(transform);
            equipment.transform.position = transform.position - new Vector3(0, 0, 10);
            equipment.GetComponent<BoxCollider2D>().enabled = false;
            equipment.GetComponent<WeaponControl>().weaponIsEquiped = true;
        }
        if(isServer)
        {
            Vector2 direction = new Vector2(target.transform.position.x - gameObject.transform.position.x, target.transform.position.y - gameObject.transform.position.y);
            AimPlayer(direction);
            LocalWeaponRotate(direction);
            CmdWeaponRotate(sortingOrderOfWeapon, adjustWeaponToward, angle);
            Fire();
        }
        else
        {
            WeaponRotate();
        }
    }

    public void AimPlayer(Vector2 direction)
    {
        if (Vector3.Angle(Vector3.up, direction) <= 45f)
        {
            enemyTowards = ENEMY_TOWARDS.UP;
        }
        if (Vector3.Angle(Vector3.down, direction) <= 45f)
        {
            enemyTowards = ENEMY_TOWARDS.DOWN;
        }
        if (Vector3.Angle(Vector3.right, direction) <= 45f)
        {
            enemyTowards = ENEMY_TOWARDS.RIGHT;
        }
        if (Vector3.Angle(Vector3.left, direction) <= 45f)
        {
            enemyTowards = ENEMY_TOWARDS.LEFT;
        }

        CmdChangeToward(enemyTowards);
    }

    [Command]
    public void CmdChangeToward(ENEMY_TOWARDS newToward)
    {
        enemyTowards = newToward;
    }

    // 武器旋转
    private void LocalWeaponRotate(Vector2 direction)
    {
        if (equipment == null)
            return;

        equipment.transform.position = this.transform.position;

        //背对着摄像机时抢在身前
        if (this.GetComponent<PlayerMovementControl>().PlayerTowards == PlayerMovementControl.PLAYER_TOWARDS.UP)
        {
            equipment.transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
            sortingOrderOfWeapon = 0;
        }
        else
        {
            equipment.transform.GetComponent<SpriteRenderer>().sortingOrder = 2;
            sortingOrderOfWeapon = 2;
        }

        //武器随鼠标旋转时朝向调整
        if (equipment.transform.eulerAngles.z <= 270 && equipment.transform.eulerAngles.z >= 90)
        {
            equipment.transform.localScale = new Vector3(1f, -1f);
            adjustWeaponToward = -1f;
        }
        else
        {
            equipment.transform.localScale = new Vector3(1f, 1f);
            adjustWeaponToward = 1f;
        }

        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
        weaponRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //根据角度进行旋转
        equipment.transform.rotation = Quaternion.Slerp(equipment.transform.rotation, weaponRotation, equipment.GetComponent<WeaponControl>().RotationSpeed * Time.deltaTime);
    }
    [Command]
    private void CmdWeaponRotate(int newOrder, float newToward, float newAngle)
    {
        sortingOrderOfWeapon = newOrder;
        adjustWeaponToward = newToward;
        angle = newAngle;
    }
    private void WeaponRotate()
    {
        equipment.transform.position = transform.position;
        equipment.transform.GetComponent<SpriteRenderer>().sortingOrder = sortingOrderOfWeapon;
        equipment.transform.localScale = new Vector3(1f, adjustWeaponToward);
        weaponRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        equipment.transform.rotation = Quaternion.Slerp(equipment.transform.rotation, weaponRotation, equipment.GetComponent<WeaponControl>().RotationSpeed * Time.deltaTime);
    }

    private void Fire()
    {
        equipment.GetComponent<WeaponControl>().Trigger();
        equipment.GetComponent<WeaponControl>().UnTrigger();
    }
}
