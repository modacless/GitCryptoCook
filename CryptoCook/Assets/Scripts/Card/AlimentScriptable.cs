using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAliment", menuName = "Cards/New aliment")]
public class AlimentScriptable : ScriptableCard
{
    public enum AlimentType
    {
        Légume,
        Fruit,
        Viande,
        Féculent,
        Poisson
    }

    public AlimentType alimentType;

    public enum Gout
    {
        Salé,
        Sucré,
        Epicé,
        Acide,
        Amer
    }

    public Gout gout;
}
