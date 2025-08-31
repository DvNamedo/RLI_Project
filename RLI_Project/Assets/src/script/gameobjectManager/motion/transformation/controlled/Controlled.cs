using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controlled : MonoBehaviour, ITransformation
{
    public abstract int Order { get; set; }

    public abstract void Accumulate(ref MotionContext context);
}
