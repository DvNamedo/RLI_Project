using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Biom
{
    Base,
    CityOfSexyGuy,
    None
}

public abstract class BiomeBase
{
    public string name { get; set; }
    public abstract Biom Id { get; }

}

//example
public abstract class CityOfSexyGuy : BiomeBase
{
    public override Biom Id { get; } = Biom.CityOfSexyGuy;
}