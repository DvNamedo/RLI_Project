using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum dir 
{
    up,
    right,
    down,
    left,
    none
}

public enum RoomTag
{
    main,
    sub
}

public abstract class RoomBase
{
    public RoomBase ReferencedBy = null;
    public Vector2 Position { get; set; }
    public BiomeBase Biome { get; set; }    

    public RoomBase(Vector2 position)
    {
        Position = position; 
    
    }
}

// e,g, the back of the room , hidden shop , ...
public interface IMultiRoom
{
    // 자기 외의 다른 연결된 독립되지 않은 Room
    public RoomBase[] DifferentRooms { get; set; }
    public RoomTag tag { get; set; }

    public void sendData(RoomTag tag, byte[] data);

    public void receiveData();
}

public class IsolatedRoom : RoomBase
{
    public IsolatedRoom(Vector2 position) : base(position)
    {
        
    }
}

public class FullConnectedRoom : RoomBase
{
    public RoomBase[] Indications = new RoomBase[4] { null, null, null, null }; // up right down left

    public FullConnectedRoom(Vector2 position) : base(position) 
    {

    }
}

public class PathRoom : RoomBase
{
    public RoomBase[] Indications = new RoomBase[1] { null };

    public PathRoom(Vector2 position) : base(position)
    {
        
    }
}