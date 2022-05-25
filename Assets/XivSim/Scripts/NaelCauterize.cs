
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor.Events;
#endif

namespace AgatePris.XivSim {
    public class NaelCauterize : MonoBehaviour {
        [Serializable]
        class Cauterize {
            [SerializeField] Toggle toggle;
            [SerializeField] LineRenderer lineRenderer;
            public void OnValueChanged() => lineRenderer.enabled = toggle.isOn;
            public Cauterize(Toggle toggle, LineRenderer lineRenderer) {
                this.toggle = toggle;
                this.lineRenderer = lineRenderer;
            }
        }

        [Serializable]
        class Dragon {
            [SerializeField] SpriteRenderer spriteRenderer;
            [SerializeField] List<Toggle> toggles;
            public void OnValueChanged() {
                var alpha = toggles.Any(toggle => toggle.isOn) ? 1.0f : 0.5f;
                var color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
            public Dragon(SpriteRenderer spriteRenderer, List<Toggle> toggles) {
                this.spriteRenderer = spriteRenderer;
                this.toggles = toggles;
            }
        }

        [SerializeField] bool validate;
        [SerializeField] Dropdown dragonPositionPreset;
        [SerializeField] List<LineRenderer> lines;
        [SerializeField] List<Dragon> dragons;
        [SerializeField] List<Cauterize> cauterizes;

        public void OnCauterizeToggle(int index) {
            cauterizes[index].OnValueChanged();
            foreach (var dragon in dragons) {
                dragon.OnValueChanged();
            }
        }

        void ValidateDragons() {
            var spriteRenderers = new List<SpriteRenderer>();
            transform.Find("Dragons").GetComponentsInChildren(spriteRenderers);

            var allToggles = new List<Toggle>();
            transform.Find("Canvas/Cauterize Toggles").GetComponentsInChildren(allToggles);

            dragons = spriteRenderers.Select((spriteRenderer, index) => {
                var toggles = index switch {
                    0 => allToggles.Take(1).ToList(),
                    1 => allToggles.Skip(1).Take(1).ToList(),
                    2 => allToggles.Skip(2).Take(2).ToList(),
                    3 => allToggles.Skip(4).Take(3).ToList(),
                    4 => allToggles.Skip(7).Take(3).ToList(),
                    5 => allToggles.Skip(10).Take(3).ToList(),
                    6 => allToggles.Skip(13).Take(1).ToList(),
                    7 => allToggles.Skip(14).Take(1).ToList(),
                    _ => null
                };
                return new Dragon(spriteRenderer, toggles);
            }).ToList();
        }
        void ValidateCauterizes() {
            cauterizes.Clear();

            var toggles = new List<Toggle>();
            transform.Find("Canvas/Cauterize Toggles").GetComponentsInChildren(toggles);

            var lineRenderers = new List<LineRenderer>();
            transform.Find("Cauterizes").GetComponentsInChildren(lineRenderers);

            toggles
                .Zip(lineRenderers, (toggle, lineRenderer) => (toggle, lineRenderer))
                .Select((tuple, index) => {
                    var toggle = tuple.toggle;
                    var lineRenderer = tuple.lineRenderer;
#if UNITY_EDITOR
                    while (0 < toggle.onValueChanged.GetPersistentEventCount()) {
                        UnityEventTools.RemovePersistentListener(toggle.onValueChanged, 0);
                    }
                    UnityEventTools.AddIntPersistentListener(
                        toggle.onValueChanged,
                        OnCauterizeToggle,
                        index);
#endif
                    toggle.onValueChanged.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
                    cauterizes.Add(new Cauterize(toggle, lineRenderer));
                    return true;
                })
                .ToList();
        }
        void OnValidate() {
            if (!validate) {
                return;
            }
            ValidateDragons();
            ValidateCauterizes();
        }
    }
}
