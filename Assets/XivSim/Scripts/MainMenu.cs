using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AgatePris.XivSim {
    public class MainMenu : MonoBehaviour {
        [SerializeField] List<GameObject> gameObjects;
        public void OnValueChanged(int value) => gameObjects.Select((gameObject, index) => {
            gameObject.SetActive(value == index);
            return false;

        }).ToList();
        void Start() => OnValueChanged(0);
    }
}
