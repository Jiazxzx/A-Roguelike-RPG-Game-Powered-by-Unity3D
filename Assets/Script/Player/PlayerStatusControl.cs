using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

//控制并存储角色的不同状态、属性

public class PlayerStatusControl : NetworkBehaviour
{

    public float damageTime = 0.5f; //角色收到攻击的时间间隔
    public int MaxHP = 10; //最大HP
    public int InitHP = 10; //初始HP

    [SyncVar(hook = "OnChangeHP")]
    public int HP = 0; //当前HP

    public int MaxAmmo = 500; //最大弹药量
    public int InitAmmo = 100;
    public int ammo = 0; //当前弹药量



    [SyncVar]
    private bool IsHit = false; //是否为被击中状态
    [SyncVar]
    public bool IsDead = false; //是否死亡
    [SyncVar]
    public bool IsHidden = false; //是否为躲藏状态

    private float flashTimer;
    private SpriteRenderer spriteRender;

    [SyncVar(hook = "OnGameWin")]
    public bool gameWin = false;
    [SyncVar(hook = "OnGameLost")]
    public bool gameLost = false;


    // 修改部分
    public void OnGameWin(bool isWin)
    {
        if (!isLocalPlayer)
            return;

        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Scene_win");
    }

    public void OnGameLost(bool isLost)
    {
        if (!isLocalPlayer)
            return;

        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Scene_lost");
    }

    //变为躲藏状态
    [Command]
    public void CmdSetHidden()
    {
        IsHidden = true;
    }
    //变为非躲藏状态
    [Command]
    public void CmdSetUnHidden()
    {
        IsHidden = false;
    }

    [Command]
    void CmdDie(GameObject diePlayer)
    {
        diePlayer.GetComponent<PlayerStatusControl>().RpcDie();
        IsDead = true;
    }

    [ClientRpc]
    public void RpcDie()
    {
        Debug.Log("S:" + isServer + "   C:" + isLocalPlayer);
        if (isLocalPlayer)
        {
            Debug.Log("in the Rpc");
            transform.position = Vector3.zero;
        }
    }

    // ///////////////////////////////
    void OnChangeHP(int newHP)
    {
        HP = newHP;
    }

    //返回角色是否死亡
    public bool isDead()
    {
        return IsDead;
    }

    //返回角色是否是被击中状态
    public bool isHit()
    {
        return IsHit;
    }

    //返回角色当前HP
    public int getCurrentHP()
    {
        return HP;
    }

    //对角色造成伤害
    public void damagePlayer(int amount)
    {
        Debug.Log("damage");
        if (!isServer) return;

        if (IsHit == false)
        {
            IsHit = true;
            HP -= amount;
            //死否死亡
            if (HP <= 0)
            {
                Debug.Log("Die");
                IsDead = true;
                HP = MaxHP;
                ammo = 100;
                RpcDie();
            }
        }
    }

    public void healPlayer(int amount)
    {
        Debug.Log("heal");
        if (!isServer) return;

        HP += amount;
        if (HP >= MaxHP)
        {
            HP = MaxHP;
        }
    }

    //复活
    public void revive()
    {
        HP = MaxHP;
        IsDead = false;
    }

    //角色增加弹药
    public void addAmmo(int amount)
    {
        ammo += amount;
        if (ammo >= MaxAmmo)
        {
            ammo = MaxAmmo;
        }
    }

    //角色消耗弹药
    public bool consumeAmmo(int amount)
    {
        if (ammo >= amount)
        {
            ammo -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    //返回当前弹药量
    public int getCurrentAmmo()
    {
        return ammo;
    }

    // Use this for initialization
    void Start()
    {
        HP = InitHP;
        ammo = InitAmmo;
        IsDead = false;
        flashTimer = damageTime;
        spriteRender = this.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //击中效果
        if (IsHit)
        {
            flashTimer -= Time.deltaTime;

            if (flashTimer > damageTime - 0.1f)
            {
                spriteRender.color = new Color(0.9f, 0.5f, 0.5f, 1f);
            }
            else if (flashTimer > 0 && flashTimer <= damageTime - 0.1f)
            {
                spriteRender.color = new Color(1f, 1f, 1f, 1f);
            }
            else if (flashTimer < 0)
            {
                IsHit = false;
                flashTimer = damageTime;
            }
        }
    }
}
