using System;
using UnityEngine;

[Serializable] public class TileData
{
    //JSONdaki veriler
    public int step;
    public bool is_empty;
    public int fruit;
    public int amount;
}
[Serializable] public class MapData
{
    public TileData[] node_list;
}