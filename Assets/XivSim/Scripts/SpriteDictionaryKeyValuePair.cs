
using System;
using UnityEngine;

[Serializable]
public struct SpriteDictionaryKeyValuePair {
    [SerializeField] string key;
    [SerializeField] Sprite value;

    public string Key {
        get => key;
        set { key = value; }
    }
    public Sprite Value {
        get => value;
        set { this.value = value; }
    }
}
