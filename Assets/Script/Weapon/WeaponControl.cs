using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// 武器道具的统一脚本
/// 包含武器的属性
/// 包括武器的伤害值、弹药消耗量、攻击发动
/// 控制武器的操作
/// 包括拾取武器、武器旋转、射击
/// </summary>

abstract public class WeaponControl : Item
{
    // 固定属性，负责本地判断
    public float RotationSpeed = 10f; //武器跟随鼠标旋转速度，可表示武器重量的影响
    public float AttackSpeed = 0.2f;//攻击速度，时间间隔
    public int DamagePerHit = 1; //武器的单发伤害值
    public bool NeedAmmo = true; //武器是否需要弹药
    public int AmmoNeeded = 1; //武器消耗的弹药量

    public AudioClip gunfireAudio;
    public AudioClip dryfireAudio;
    protected AudioSource m_audio; //声音源

    public GameObject player; //玩家
    public bool weaponIsEquiped = false; //武器是否被装备上
    [SyncVar] private float AttackTimer = 0;//攻击速度计时器
    [SyncVar] private bool isTriggered; //武器是否正在攻击

    void Awake()
    {
        weaponIsEquiped = false;
        isTriggered = false;
        m_audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        //武器攻击时间间隔计时
        AttackTimer -= Time.deltaTime;
        if (AttackTimer <= 0 && isTriggered)
        {
            if (player.tag == "monster")
            {
                WeaponTriggered(DamagePerHit);
                //射击声音
                m_audio.PlayOneShot(gunfireAudio);
            }
            //如果弹药足够，攻击一次
            else if(player && player.GetComponent<PlayerStatusControl>().consumeAmmo(AmmoNeeded))
            {
                
                WeaponTriggered(DamagePerHit);
                //射击声音
                m_audio.PlayOneShot(gunfireAudio);
            }
            else
            {
                m_audio.PlayOneShot(dryfireAudio);
            }

            //重置计时器
            AttackTimer = AttackSpeed;
            Debug.Log("fire!!!" + AttackTimer);
        }
    }

    // 虚函数,武器攻击时执行的内容
    public abstract void WeaponTriggered(int damage);
    public void Trigger()
    {
        isTriggered = true;
        CmdSyncTrigger(isTriggered);
    }
    public void UnTrigger()
    {
        isTriggered = false;
        CmdSyncTrigger(isTriggered);
    }
    [Command]
    public void CmdSyncTrigger(bool trigger)
    {
        isTriggered = trigger;

    }

    //拾取武器
    public override void interact(GameObject other)
    {
        player = other;
        //玩家为装备武器则拾取武器
        if (!weaponIsEquiped)
        {
            other.GetComponent<PlayerEquipmentControl>().TryEquipWeapon(gameObject.GetComponent<NetworkIdentity>());
        }
    }
}
