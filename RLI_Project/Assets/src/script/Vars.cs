using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class Vars
{
    private static int recentMapNodeID = 0;
    public static int NewMapNodeID
    {

        get => Interlocked.Increment(ref recentMapNodeID); // get safety in multithreading environment
        private set => value = recentMapNodeID;
    }

}
