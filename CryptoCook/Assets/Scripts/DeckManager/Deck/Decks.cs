using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
public class Decks : MonoBehaviour
{
    [Serializable]
    public struct Deck
    {
        public string name;
        public List<ChefCardScriptable> allCards;
    }
    [SerializeField]
    private Deck[] dataDecks;
    public Dictionary<string, List<ChefCardScriptable>> decks;

    public void Awake()
    {
        decks = new Dictionary<string, List<ChefCardScriptable>>();

        for(int i =0; i< dataDecks.Length; i++)
        {
            decks.Add(dataDecks[i].name, dataDecks[i].allCards);
        }
        
        
    }


}
