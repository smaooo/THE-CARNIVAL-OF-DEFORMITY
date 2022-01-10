using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Order", menuName = "Order")]

public class LayerOrders : ScriptableObject
{
    [System.Serializable]
    public enum Locations { First, Second, Third, Left, Right, Middle, Front1, Front2, Back1, Back2, Left1, Left2, Right1, Right2}
    [System.Serializable]
    public struct InsideLayers
    {
        public string name;
        public int locs;
    }
    [System.Serializable]
    public struct Obj
    {
        public string name;
        public int objLoc;
        public InsideLayers[] locs;
    }
    [System.Serializable]
    public struct Layer
    {
        public string name;
        public Obj[] objLocs;
    }
    [System.Serializable]
    public struct Order
    {
        public string name;
        public Layer[] layer;
        public Locations location1;
        public Locations location2;
    }

    public Order[] orders;
}
