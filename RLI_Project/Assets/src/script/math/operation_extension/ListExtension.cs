using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtension
{
    /// <summary>
    /// UnityEngine.Random을 쓰는 제자리 셔플. 전역 RNG 상태에 의존(재현성 X).
    /// </summary>
    public static void ShuffleInPlace<T>(this IList<T> list)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        for (int i = list.Count - 1; i > 0; --i)
        {
            // int용 Range는 상한 배타. [0, i]에서 뽑으려면 i+1.
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static List<T> Shuffled<T>(this IEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        var list = new List<T>(source);
        list.ShuffleInPlace();
        return list;
    }


    /// <summary>
    /// 재현성 있는 제자리 셔플. System.Random 주입(권장).
    /// </summary>
    public static void ShuffleInPlace<T>(this IList<T> list, System.Random rng)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (rng == null) throw new ArgumentNullException(nameof(rng));
        for (int i = list.Count - 1; i > 0; --i)
        {
            int j = rng.Next(i + 1); // [0, i]
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
