using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// 负责控制角色手上的武器
/// 武器攻击、旋转
/// </summary>

public class PlayerEquipmentControl : NetworkBehaviour
{

    public GameObject Equipment;                    //武器对象
    [SyncVar] public bool IsEquip = false;          //判断玩家是否持有武器
    public bool CanPick;                            //是否能够拾取武器
    public float PickTime = 2f;                     //角色拾取武器时间间隔
    private float PickTimer;                        //拾取间隔计时器
    private Quaternion weaponRotation;              //武器旋转方向
    private rightJoystick rjs;                      //摇杆

    // Use this for initialization
    void Start()
    {
        rjs = GetComponent<rightJoystick>();
        PickTimer = PickTime;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (IsEquip)
            {
                LocalWeaponRotate();
                CmdWeaponRotate(sortingOrderOfWeapon, adjustWeaponToward, angle);
                Fire();
            }
            if (!CanPick)
            {
                PickTimer -= Time.deltaTime;
                if (PickTimer <= 0)
                {
                    CanPick = true;
                    PickTimer = PickTime;
                }
            }
        }
        else
        {
            if (IsEquip)
                WeaponRotate();
        }
    }

    [SyncVar] int sortingOrderOfWeapon = 0;
    [SyncVar] float adjustWeaponToward = 1f;
    [SyncVar] float angle = 0;

    // 触发确认武器射击
    private void Fire()
    {
        if (Equipment == null) return;
        //键盘射击方案
        if (GetComponent<PlayerMovementControl>().controlMode == PlayerMovementControl.CONTROL_MODE.Keyboard)
        {

            if (Input.GetMouseButton(0))
            {
                Debug.Log("Weapon Trigger");
                Equipment.GetComponent<WeaponControl>().Trigger();
            }
            else
            {
                Equipment.GetComponent<WeaponControl>().UnTrigger();
            }
        }

        //摇杆射击方案
        if (GetComponent<PlayerMovementControl>().controlMode == PlayerMovementControl.CONTROL_MODE.TouchScreen)
        {
            //得到摇杆的向量
            float rightValueX = rjs.direction.x;
            float rightValueY = rjs.direction.y;

            //射击
            var Xm = Mathf.Abs(rightValueX);
            var Ym = Mathf.Abs(rightValueY);
            if (Xm > 0.8 || Ym > 0.8 ||( Xm + Ym>0.8))
            {
                Equipment.GetComponent<WeaponControl>().Trigger();
            }
            else
            {
                Equipment.GetComponent<WeaponControl>().UnTrigger();
            }
        }
    }

    // 武器旋转
    private void LocalWeaponRotate()
    {
        if (Equipment == null)
            return;

        Equipment.transform.position = this.transform.position;

        //背对着摄像机时抢在身前
        if (this.GetComponent<PlayerMovementControl>().PlayerTowards == PlayerMovementControl.PLAYER_TOWARDS.UP)
        {
            Equipment.transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
            sortingOrderOfWeapon = 0;
        }
        else
        {
            Equipment.transform.GetComponent<SpriteRenderer>().sortingOrder = 2;
            sortingOrderOfWeapon = 2;
        }

        //武器随鼠标旋转时朝向调整
        if (Equipment.transform.eulerAngles.z <= 270 && Equipment.transform.eulerAngles.z >= 90)
        {
            Equipment.transform.localScale = new Vector3(1f, -1f);
            adjustWeaponToward = -1f;
        }
        else
        {
            Equipment.transform.localScale = new Vector3(1f, 1f);
            adjustWeaponToward = 1f;
        }

        //键盘旋转
        if (GetComponent<PlayerMovementControl>().controlMode == PlayerMovementControl.CONTROL_MODE.Keyboard)
        {
            //得到指向鼠标的向量
            Vector2 direction = rjs.playerCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            //计算角度（tan = y/x）
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
            weaponRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //根据角度进行旋转
            Equipment.transform.rotation = Quaternion.Slerp(Equipment.transform.rotation, weaponRotation, Equipment.GetComponent<WeaponControl>().RotationSpeed * Time.deltaTime);
        }

        //摇杆旋转方案
        if (GetComponent<PlayerMovementControl>().controlMode == PlayerMovementControl.CONTROL_MODE.TouchScreen)
        {
            //得到摇杆的向量
            Vector2 direction = rjs.direction;
            //计算角度（tan = y/x）
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
            weaponRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //根据角度进行旋转
            Equipment.transform.rotation = Quaternion.Slerp(Equipment.transform.rotation, weaponRotation, Equipment.GetComponent<WeaponControl>().RotationSpeed * Time.deltaTime);
        }
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
        Equipment.transform.position = transform.position;
        Equipment.transform.GetComponent<SpriteRenderer>().sortingOrder = sortingOrderOfWeapon;
        Equipment.transform.localScale = new Vector3(1f, adjustWeaponToward);
        weaponRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Equipment.transform.rotation = Quaternion.Slerp(Equipment.transform.rotation, weaponRotation, Equipment.GetComponent<WeaponControl>().RotationSpeed * Time.deltaTime);
    }


    // 装备武器
    public void TryEquipWeapon(NetworkIdentity weaponID)
    {
        if (!isLocalPlayer)
            return;
        if (ClientScene.FindLocalObject(weaponID.netId).GetComponent<WeaponControl>().weaponIsEquiped)
            return;
        if (!CanPick)
            return;

        if (IsEquip)
        {
            CmdUnequipWeapon();
        }

        CanPick = false;
        CmdEquipWeapon(weaponID);
    }

    // 角色装上武器
    [Command]
    public void CmdEquipWeapon(NetworkIdentity weaponID)
    {
        weaponID.AssignClientAuthority(connectionToClient);
        RpcEquipWeapon(weaponID);
        IsEquip = true;
    }
    [ClientRpc]
    public void RpcEquipWeapon(NetworkIdentity weaponID)
    {
        GameObject localWeapon = ClientScene.FindLocalObject(weaponID.netId) as GameObject;
        localWeapon.transform.SetParent(transform);
        localWeapon.transform.position = transform.position - new Vector3(0, 0, 10);
        localWeapon.GetComponent<BoxCollider2D>().enabled = false;
        Equipment = localWeapon;
        Equipment.GetComponent<WeaponControl>().weaponIsEquiped = true;
    }

    // 角色卸下武器
    [Command]
    public void CmdUnequipWeapon()
    {
        Equipment.GetComponent<NetworkIdentity>().RemoveClientAuthority(connectionToClient);
        RpcUnequipWeapon();
        IsEquip = false;
    }
    [ClientRpc]
    public void RpcUnequipWeapon()
    {
        Equipment.GetComponent<WeaponControl>().weaponIsEquiped = false;
        Equipment.GetComponent<BoxCollider2D>().enabled = true;
        Equipment.transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
        Equipment.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        Equipment.transform.position = transform.position + Vector3.down;
        Equipment.transform.SetParent(null);
        Equipment = null;
    }

    public bool isEquip()
    {
        return IsEquip;
    }
}