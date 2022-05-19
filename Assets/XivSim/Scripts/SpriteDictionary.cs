
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpriteDictionary {
    [SerializeField] List<SpriteDictionaryKeyValuePair> list;
    public Sprite Get(string key) {
        foreach (var e in list) {
            if (e.Key == key) {
                return e.Value;
            }
        }
        return null;
    }
}
