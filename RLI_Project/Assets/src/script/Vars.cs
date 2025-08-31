using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Provides a independant from unity and thread-safe mechanism <br></br>
/// e.g. to generate unique identifiers for map nodes.
/// </summary>
/// <remarks>The <see cref="NewMapNodeID"/> property generates a new unique identifier each time it is accessed.
/// This ensures that the identifiers are sequential and thread-safe, making it suitable for use in multi-threaded
/// environments.</remarks>
public static class Vars
{
    private static int recentMapNodeID = 0;
    public static int NewMapNodeID
    {

        get => Interlocked.Increment(ref recentMapNodeID); // get safety in multithreading environment
        private set => value = recentMapNodeID;
    }

}
