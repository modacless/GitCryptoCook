using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAliment", menuName = "Cards/New aliment")]
public class AlimentScriptable : ScriptableCard
{
    public enum AlimentType
    {
        L�gume,
        Fruit,
        Viande,
        F�culent,
        Poisson
    }

    public AlimentType alimentType;

    public enum Gout
    {
        Sal�,
        Sucr�,
        Epic�,
        Acide,
        Amer
    }

    public Gout gout;
}
