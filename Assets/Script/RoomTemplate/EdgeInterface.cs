using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEdgeInterface{
    List<Vector2>[] GetRoomEdgeAndInterier(int roomNumber);
        //return a array with length of 9.
        //result[0] = up;
        //result[1] = down;
        //result[2] = left;
        //result[3] = right;
        //result[4] = leftup;
        //result[5] = rightup;
        //result[6] = leftdown;
        //result[7] = rightdown;
        //result[8] = inter;
        //result[9] = hallway;
        //result[10] = door;
}
