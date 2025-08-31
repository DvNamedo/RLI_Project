using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity2D : NonControlled
{
    [Header("Gravity2D Settings")]
    [Range(-30f, 30f)]
    public float gravity = 9.8f;

    [Header("Excution Order")]
    public int m_Order;

    override public int Order
    {
        get
        {
            return m_Order;
        }
        set
        {
            value = m_Order;
        }
    }

    public override void Accumulate(ref MotionContext context)
    {
        context.acceleration += Vector2.down * gravity;
        context.velocity += context.acceleration * Time.fixedDeltaTime;
        context.position += context.velocity * Time.fixedDeltaTime; // ¹®Á¦ êó
    }

    private void FixedUpdate()
    {
        
    }

}
