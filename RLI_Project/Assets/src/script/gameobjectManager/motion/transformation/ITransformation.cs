using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransformation
{
    int Order { get; set; }

    void Accumulate(ref MotionContext context);
}
