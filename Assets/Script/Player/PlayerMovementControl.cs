using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovementControl : NetworkBehaviour
{
    public enum PLAYER_TOWARDS { UP, DOWN, RIGHT, LEFT }; //角色朝向
    public enum CONTROL_MODE { TouchScreen, Keyboard }; //角色操控方式

    public float Mspeed = 1f; //移动速度

    private float speedEnlarge = 0.15f;
    private Vector3 movement; //移动向量

    private Vector3 rush; //冲刺方向
    private float rushTimer; //冲刺计时器
    public float rushTime = 0.5f; //冲刺持续时间
    private bool isRushing = false;

    private Animator animator; //
    private Rigidbody2D rb2d;
    private SpriteRenderer SpriteRender;

    private leftJoystick leftjoy;
    private rightJoystick rightjoy;


    public CONTROL_MODE controlMode;

    [SyncVar]
    public PLAYER_TOWARDS PlayerTowards;

    protected Transform m_transform;

    // Use this for initialization
    void Start()
    {
        m_transform = this.transform;
        animator = this.gameObject.GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        SpriteRender = GetComponent<SpriteRenderer>();
        PlayerTowards = PLAYER_TOWARDS.DOWN;
        rushTimer = rushTime;
        leftjoy = GetComponent<leftJoystick>();
        rightjoy = GetComponent<rightJoystick>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (controlMode == CONTROL_MODE.Keyboard)
        {
            Rotate();
            Move();
            //Rush();
        }
        if (controlMode == CONTROL_MODE.TouchScreen)
        {
            Rotate(rightjoy.direction);
            Move(leftjoy.direction);
        }
    }

    [Command]
    public void CmdChnageToward(PLAYER_TOWARDS newToward)
    {
        PlayerTowards = newToward;
    }

    //鼠标旋转方案
    public void Rotate()
    {
        //得到指向鼠标的向量
        Vector2 direction = rightjoy.playerCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        if (Vector3.Angle(Vector3.up, direction) <= 45f)
        {
            animator.SetInteger("playerTowards", 0);
            PlayerTowards = PLAYER_TOWARDS.UP;
        }
        if (Vector3.Angle(Vector3.down, direction) <= 45f)
        {
            animator.SetInteger("playerTowards", 1);
            PlayerTowards = PLAYER_TOWARDS.DOWN;
        }
        if (Vector3.Angle(Vector3.right, direction) <= 45f)
        {
            animator.SetInteger("playerTowards", 2);
            PlayerTowards = PLAYER_TOWARDS.RIGHT;
        }
        if (Vector3.Angle(Vector3.left, direction) <= 45f)
        {
            animator.SetInteger("playerTowards", 3);
            PlayerTowards = PLAYER_TOWARDS.LEFT;
        }

        CmdChnageToward(PlayerTowards);
    }

    //摇杆旋转方案
    public void Rotate(Vector3 direction)
    {
        //得到摇杆的向量   
        if (Vector3.Angle(Vector3.up, direction) <= 45f)
        {
            animator.SetInteger("playerTowards", 0);
            PlayerTowards = PLAYER_TOWARDS.UP;
        }
        if (Vector3.Angle(Vector3.down, direction) <= 45f)
        {
            animator.SetInteger("playerTowards", 1);
            PlayerTowards = PLAYER_TOWARDS.DOWN;
        }
        if (Vector3.Angle(Vector3.right, direction) <= 45f)
        {
            animator.SetInteger("playerTowards", 2);
            PlayerTowards = PLAYER_TOWARDS.RIGHT;
        }
        if (Vector3.Angle(Vector3.left, direction) <= 45f)
        {
            animator.SetInteger("playerTowards", 3);
            PlayerTowards = PLAYER_TOWARDS.LEFT;
        }

        CmdChnageToward(PlayerTowards);
    }

    //键盘移动方案
    public void Move()
    {
        float h = 0;
        float v = 0;

        //获取移动方向
        if (Input.GetKey(KeyCode.W))
        {
            v = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            v = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            h = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            h = 1f;
        }

        //是否触发移动动画
        if (h == 0 && v == 0)
        {
            animator.SetInteger("isWalking", 0);
        }
        else
        {
            animator.SetInteger("isWalking", 1);
        }

        //移动角色位置
        movement = new Vector3(h, v, 0f);
        rb2d.MovePosition(transform.position + movement * Mspeed * 0.1f);


        //触发角色冲刺
        if (!isRushing && Input.GetKeyDown(KeyCode.Space) && (h != 0 || v != 0))
        {
            animator.SetBool("isRushing", true);
            isRushing = true;
        }

        //冲刺过程
        if (isRushing)
        {
            rushTimer -= Time.deltaTime;

            if (rushTimer >= rushTime - 0.2f)
            {
                rb2d.MovePosition(transform.position + movement * 0.3f);
            }

            if (rushTimer < rushTime - 0.2f && rushTimer >= 0)
            {
                animator.SetBool("isRushing", false);
            }

            if (rushTimer < 0)
            {
                isRushing = false;
                rushTimer = rushTime;
            }
        }
    }

    //摇杆移动方案
    public void Move(Vector3 dir)
    {

        dir *= Mspeed * speedEnlarge;
        rb2d.MovePosition(transform.position + dir);

        if(Mathf.Abs(dir.x)==0 && Mathf.Abs(dir.y)==0)
            animator.SetInteger("isWalking", 0);
        else
            animator.SetInteger("isWalking", 1);

        //触发角色冲刺
        if (!isRushing && ETCInput.GetButtonDown("abilityBtn") && dir != Vector3.zero)
        {
            animator.SetBool("isRushing", true);
            isRushing = true;
        }

        //冲刺过程
        if (isRushing)
        {
            rushTimer -= Time.deltaTime;

            if (rushTimer >= rushTime - 0.2f)
            {
                rb2d.MovePosition(transform.position + dir * 2f);
            }

            if (rushTimer < rushTime - 0.2f && rushTimer >= 0)
            {
                animator.SetBool("isRushing", false);
            }

            if (rushTimer < 0)
            {
                isRushing = false;
                rushTimer = rushTime;
            }
        }
    }


}


//旧版移动方案
//void Move()
//{

//    float h = 0;
//    float v = 0;
//    //Movement
//    if (Input.GetKey(KeyCode.W))
//    {
//        v = 1f;
//        animator.SetInteger("walk", 2);
//        PlayerTowards = PLAYER_TOWARDS.UP;
//    }
//    if (Input.GetKey(KeyCode.S))
//    {
//        v = -1f;
//        animator.SetInteger("walk", -2);
//        PlayerTowards = PLAYER_TOWARDS.DOWN;
//    }
//    if (Input.GetKey(KeyCode.A))
//    {
//        h = -1f;
//        animator.SetInteger("walk", -1);
//        PlayerTowards = PLAYER_TOWARDS.LEFT;
//    }
//    if (Input.GetKey(KeyCode.D))
//    {
//        h = 1f;
//        animator.SetInteger("walk", 1);
//        PlayerTowards = PLAYER_TOWARDS.RIGHT;
//    }
//    if (h == 0 && v == 0)
//    {
//        animator.SetInteger("walk", 0);
//    }

//    movement = new Vector3(h, v, 0f);
//    movement = movement.normalized * Mspeed * speedEnlarge * Time.deltaTime;
//    rb2d.MovePosition(transform.position + movement);


//}

//public void Move(Vector3 dir)
//{

//    if (dir.x < dir.y && dir.y > 0)//W
//    {
//        animator.SetInteger("walk", 2);
//        PlayerTowards = PLAYER_TOWARDS.UP;
//    }
//    if (dir.x > dir.y && dir.y < 0)//S
//    {
//        animator.SetInteger("walk", -2);
//        PlayerTowards = PLAYER_TOWARDS.DOWN;
//    }
//    if (dir.x < dir.y && dir.x < 0)//A
//    {
//        animator.SetInteger("walk", -1);
//        PlayerTowards = PLAYER_TOWARDS.LEFT;
//    }
//    if (dir.x > dir.y && dir.x > 0)//D
//    {
//        animator.SetInteger("walk", 1);
//        PlayerTowards = PLAYER_TOWARDS.RIGHT;
//    }
//    if (dir == null || Mathf.Abs(dir.x) < 0.1f && Mathf.Abs(dir.y) < 0.1f)
//    {
//        animator.SetInteger("walk", 0);
//    }

//    dir *= Mspeed;
//    rb2d.MovePosition(transform.position + dir);

//}

//public void stay()
//{
//    animator.SetInteger("walk", 0);
//}