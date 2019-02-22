using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R11x11: RoomTemplate{
    //private void Start()
    //{
    //    InitList();
    //}

    public void InitList()
    {
        N1();
        N2();
        N3();
        N4();
        N5();
        N6();
        N7();
        N8();
        N9();
        N10();


    }
    void N1()
    {
        List<ItemAndPos> temp = new List<ItemAndPos>();
        temp.Add(new ItemAndPos(ItemType.Barrier22, 2, 7));
        temp.Add(new ItemAndPos(ItemType.Barrier22, 7, 7));
        temp.Add(new ItemAndPos(ItemType.Monster, 4, 6));
        temp.Add(new ItemAndPos(ItemType.Monster, 6, 6));
        temp.Add(new ItemAndPos(ItemType.Chest11, 5, 5));
        temp.Add(new ItemAndPos(ItemType.Monster, 4, 4));
        temp.Add(new ItemAndPos(ItemType.Monster, 6, 4));
        temp.Add(new ItemAndPos(ItemType.Barrier22, 2, 2));
        temp.Add(new ItemAndPos(ItemType.Barrier22, 7, 2));
        ItemList.Add(temp);
    }

    void N2()
    {
        List<ItemAndPos> temp = new List<ItemAndPos>();
        temp.Add(new ItemAndPos(ItemType.Barrier31, 2, 8));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 6, 6));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 2, 4));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 6, 2));
        temp.Add(new ItemAndPos(ItemType.Terrain22, 5, 7));
        temp.Add(new ItemAndPos(ItemType.Terrain22, 5, 4));
        temp.Add(new ItemAndPos(ItemType.Terrain11, 3, 2));
        temp.Add(new ItemAndPos(ItemType.Monster, 3, 5));
        temp.Add(new ItemAndPos(ItemType.Monster, 4, 6));
        temp.Add(new ItemAndPos(ItemType.Monster, 7, 5));
        ItemList.Add(temp);
    }
    void N3()
    {
        List<ItemAndPos> temp = new List<ItemAndPos>();
        temp.Add(new ItemAndPos(ItemType.Barrier21, 2, 9));
        temp.Add(new ItemAndPos(ItemType.Barrier21, 7, 9));
        temp.Add(new ItemAndPos(ItemType.Chest11, 2, 8));
        temp.Add(new ItemAndPos(ItemType.Monster, 3, 8));
        temp.Add(new ItemAndPos(ItemType.Monster, 7, 8));
        temp.Add(new ItemAndPos(ItemType.Barrier21, 2, 6));
        temp.Add(new ItemAndPos(ItemType.Barrier21, 7, 6));
        temp.Add(new ItemAndPos(ItemType.Monster, 2, 5));
        temp.Add(new ItemAndPos(ItemType.Item11, 8, 5));
        temp.Add(new ItemAndPos(ItemType.Barrier21, 2, 3));
        temp.Add(new ItemAndPos(ItemType.Barrier21, 7, 3));
        temp.Add(new ItemAndPos(ItemType.Item11, 2, 2));
        temp.Add(new ItemAndPos(ItemType.Monster, 8, 2));
        ItemList.Add(temp);
    }

    void N4()
    {
        List<ItemAndPos> temp = new List<ItemAndPos>();
        temp.Add(new ItemAndPos(ItemType.Barrier21, 2, 8));
        temp.Add(new ItemAndPos(ItemType.Item11, 4, 8));
        temp.Add(new ItemAndPos(ItemType.Item11, 6, 8));
        temp.Add(new ItemAndPos(ItemType.Barrier21, 7, 8));
        temp.Add(new ItemAndPos(ItemType.Item11, 2, 7));
        temp.Add(new ItemAndPos(ItemType.Monster, 3, 7));
        temp.Add(new ItemAndPos(ItemType.Item11, 7, 7));
        temp.Add(new ItemAndPos(ItemType.Monster, 8, 7));
        temp.Add(new ItemAndPos(ItemType.Barrier11, 5, 5));
        temp.Add(new ItemAndPos(ItemType.Barrier21, 2, 3));
        temp.Add(new ItemAndPos(ItemType.Item11, 4, 3));
        temp.Add(new ItemAndPos(ItemType.Barrier21, 7, 3));
        temp.Add(new ItemAndPos(ItemType.Item11, 9, 3));
        temp.Add(new ItemAndPos(ItemType.Item11, 2, 2));
        temp.Add(new ItemAndPos(ItemType.Monster, 3, 2));
        temp.Add(new ItemAndPos(ItemType.Item11, 7, 2));
        temp.Add(new ItemAndPos(ItemType.Monster, 8, 2));
        ItemList.Add(temp);
    }

    void N5()
    {
        List<ItemAndPos> temp = new List<ItemAndPos>();
        temp.Add(new ItemAndPos(ItemType.Monster, 0, 10));
        temp.Add(new ItemAndPos(ItemType.Item11, 2, 10));
        temp.Add(new ItemAndPos(ItemType.Item11, 3, 10));
        temp.Add(new ItemAndPos(ItemType.Item11, 4, 10));
        temp.Add(new ItemAndPos(ItemType.Item11, 5, 10));
        temp.Add(new ItemAndPos(ItemType.Monster, 10, 10));
        temp.Add(new ItemAndPos(ItemType.Item11, 0, 8));
        temp.Add(new ItemAndPos(ItemType.Monster, 4, 8));
        temp.Add(new ItemAndPos(ItemType.Item11, 0, 7));
        temp.Add(new ItemAndPos(ItemType.Terrain33, 4, 5));
        temp.Add(new ItemAndPos(ItemType.Item11, 0, 6));
        temp.Add(new ItemAndPos(ItemType.Item11, 10, 6));
        temp.Add(new ItemAndPos(ItemType.Barrier21, 3, 4));
        temp.Add(new ItemAndPos(ItemType.Item11, 10, 4));
        temp.Add(new ItemAndPos(ItemType.Item11, 0, 3));
        temp.Add(new ItemAndPos(ItemType.Monster, 3, 3));
        temp.Add(new ItemAndPos(ItemType.Monster, 6, 3));
        temp.Add(new ItemAndPos(ItemType.Item11, 10, 3));
        temp.Add(new ItemAndPos(ItemType.Monster, 0, 0));
        temp.Add(new ItemAndPos(ItemType.Item11, 3, 0));
        temp.Add(new ItemAndPos(ItemType.Item11, 4, 0));
        temp.Add(new ItemAndPos(ItemType.Item11, 5, 0));
        temp.Add(new ItemAndPos(ItemType.Item11, 6, 0));
        temp.Add(new ItemAndPos(ItemType.Monster, 10, 0));
        ItemList.Add(temp);
    }

    void N6()
    {
        List<ItemAndPos> temp = new List<ItemAndPos>();
        temp.Add(new ItemAndPos(ItemType.Barrier22, 2, 7));
        temp.Add(new ItemAndPos(ItemType.Monster, 5, 8));
        temp.Add(new ItemAndPos(ItemType.Monster, 5, 6));
        temp.Add(new ItemAndPos(ItemType.Terrain33, 7, 4));
        temp.Add(new ItemAndPos(ItemType.Monster, 2, 5));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 4, 5));
        temp.Add(new ItemAndPos(ItemType.Monster, 5, 4));
        temp.Add(new ItemAndPos(ItemType.Barrier22, 2, 2));
        temp.Add(new ItemAndPos(ItemType.Monster, 5, 2));
        ItemList.Add(temp);
    }

    void N7()
    {
        List<ItemAndPos> temp = new List<ItemAndPos>();
        temp.Add(new ItemAndPos(ItemType.Item11, 2, 10));
        temp.Add(new ItemAndPos(ItemType.Item11, 3, 10));
        temp.Add(new ItemAndPos(ItemType.Item11, 4, 10));
        temp.Add(new ItemAndPos(ItemType.Item11, 5, 10));
        temp.Add(new ItemAndPos(ItemType.Item11, 6, 10));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 1, 9));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 7, 9));
        temp.Add(new ItemAndPos(ItemType.Monster, 2, 8));
        temp.Add(new ItemAndPos(ItemType.Monster, 8, 8));
        temp.Add(new ItemAndPos(ItemType.Monster, 5, 7));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 1, 6));
        temp.Add(new ItemAndPos(ItemType.Chest11, 5, 6));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 7, 6));
        temp.Add(new ItemAndPos(ItemType.Monster, 2, 5));
        temp.Add(new ItemAndPos(ItemType.Monster, 5, 5));
        temp.Add(new ItemAndPos(ItemType.Monster, 8, 5));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 1, 3));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 7, 3));
        temp.Add(new ItemAndPos(ItemType.Monster, 2, 2));
        temp.Add(new ItemAndPos(ItemType.Monster, 8, 2));
        temp.Add(new ItemAndPos(ItemType.Item11, 4, 0));
        temp.Add(new ItemAndPos(ItemType.Item11, 5, 0));
        temp.Add(new ItemAndPos(ItemType.Item11, 6, 0));
        ItemList.Add(temp);
    }

    void N8()
    {
        List<ItemAndPos> temp = new List<ItemAndPos>();
        temp.Add(new ItemAndPos(ItemType.Barrier31, 3, 9));
        temp.Add(new ItemAndPos(ItemType.Chest11, 3, 8));
        temp.Add(new ItemAndPos(ItemType.Item11, 4, 8));
        temp.Add(new ItemAndPos(ItemType.Item11, 5, 8));
        temp.Add(new ItemAndPos(ItemType.Monster, 6, 4));
        temp.Add(new ItemAndPos(ItemType.Monster, 8, 4));
        temp.Add(new ItemAndPos(ItemType.Item11, 9, 3));
        temp.Add(new ItemAndPos(ItemType.Item11, 10, 3));
        temp.Add(new ItemAndPos(ItemType.Monster, 6, 2));
        temp.Add(new ItemAndPos(ItemType.Barrier21, 7, 2));
        temp.Add(new ItemAndPos(ItemType.Item11, 9, 2));
        temp.Add(new ItemAndPos(ItemType.Item11, 10, 2));
        temp.Add(new ItemAndPos(ItemType.Item11, 7, 1));
        temp.Add(new ItemAndPos(ItemType.Item11, 8, 1));
        temp.Add(new ItemAndPos(ItemType.Item11, 9, 1));
        temp.Add(new ItemAndPos(ItemType.Item11, 10, 1));
        temp.Add(new ItemAndPos(ItemType.Item11, 7, 0));
        temp.Add(new ItemAndPos(ItemType.Item11, 8, 0));
        temp.Add(new ItemAndPos(ItemType.Item11, 9, 0));
        temp.Add(new ItemAndPos(ItemType.Item11, 10, 0));
        ItemList.Add(temp);
    }

    void N9()
    {
        List<ItemAndPos> temp = new List<ItemAndPos>();
        temp.Add(new ItemAndPos(ItemType.Barrier71, 2, 1));
        temp.Add(new ItemAndPos(ItemType.Barrier71, 2, 9));
        temp.Add(new ItemAndPos(ItemType.Barrier15, 9, 3));
        temp.Add(new ItemAndPos(ItemType.Barrier15, 1, 3));
        temp.Add(new ItemAndPos(ItemType.Monster, 3, 6));
        temp.Add(new ItemAndPos(ItemType.Terrain33, 4, 4));
        temp.Add(new ItemAndPos(ItemType.Monster, 7, 6));
        temp.Add(new ItemAndPos(ItemType.Monster, 3, 4));
        temp.Add(new ItemAndPos(ItemType.Monster, 7, 4));
        temp.Add(new ItemAndPos(ItemType.Chest11, 5, 2));
        ItemList.Add(temp);
    }

    void N10()
    {
        List<ItemAndPos> temp = new List<ItemAndPos>();
        temp.Add(new ItemAndPos(ItemType.Barrier31, 2, 9));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 5, 9));
        temp.Add(new ItemAndPos(ItemType.Barrier11, 8, 9));
        temp.Add(new ItemAndPos(ItemType.Item11, 2, 8));
        temp.Add(new ItemAndPos(ItemType.Item11, 3, 8));
        temp.Add(new ItemAndPos(ItemType.Item11, 4, 8));
        temp.Add(new ItemAndPos(ItemType.Item11, 5, 8));
        temp.Add(new ItemAndPos(ItemType.Item11, 6, 8));
        temp.Add(new ItemAndPos(ItemType.Item11, 7, 8));
        temp.Add(new ItemAndPos(ItemType.Item11, 8, 8));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 2, 5));
        temp.Add(new ItemAndPos(ItemType.Barrier31, 5, 5));
        temp.Add(new ItemAndPos(ItemType.Barrier11, 8, 5));
        temp.Add(new ItemAndPos(ItemType.Item11, 2, 4));
        temp.Add(new ItemAndPos(ItemType.Item11, 3, 4));
        temp.Add(new ItemAndPos(ItemType.Item11, 4, 4));
        temp.Add(new ItemAndPos(ItemType.Item11, 5, 4));
        temp.Add(new ItemAndPos(ItemType.Item11, 6, 4));
        temp.Add(new ItemAndPos(ItemType.Item11, 7, 4));
        temp.Add(new ItemAndPos(ItemType.Item11, 8, 4));
        temp.Add(new ItemAndPos(ItemType.Barrier21, 2, 2));
        temp.Add(new ItemAndPos(ItemType.Chest11, 5, 2));
        temp.Add(new ItemAndPos(ItemType.Barrier21, 7, 2));
        temp.Add(new ItemAndPos(ItemType.Item11, 2, 1));
        temp.Add(new ItemAndPos(ItemType.Item11, 3, 1));
        temp.Add(new ItemAndPos(ItemType.Item11, 7, 1));
        temp.Add(new ItemAndPos(ItemType.Item11, 8, 1));
        ItemList.Add(temp);
    }



}
