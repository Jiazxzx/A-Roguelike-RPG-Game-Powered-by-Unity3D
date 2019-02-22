using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 一切可互动物品基类
/// 具体物体实现该类并overrideinteract以及OnChangeState
/// </summary>

public abstract class Item : NetworkBehaviour
{
    //物体互动状态
    [SyncVar(hook = "OnChangeState")]
    public bool state;

    //物体互动函数接口
    //interact负责对物品状态state做出改变
    //同时获取互动主体对象
    //不同的物品状态改变的规则逻辑通过实现interact来完成
    public abstract void interact(GameObject player);

    //状态改变时所执行的事件
    //实现OnChangeState来根据state使物体做出具体行为
    public abstract void OnChangeState(bool state);
}
