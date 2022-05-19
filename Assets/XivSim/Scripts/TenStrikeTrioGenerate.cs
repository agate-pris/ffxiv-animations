
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor.Events;
#endif

namespace AgatePris.XivSim {
    public class TenStrikeTrioGenerate : MonoBehaviour {
        [Serializable]
        class Player {
            [SerializeField] SpriteRenderer spriteRenderer;
            [SerializeField] List<LineRenderer> arrows;
            [SerializeField] Toggle toggle;
            public SpriteRenderer SpriteRenderer => spriteRenderer;
            public List<LineRenderer> Arrows => arrows;
            public Toggle Toggle => toggle;
            public Player(SpriteRenderer spriteRenderer, List<LineRenderer> arrows, Toggle toggle) {
                this.spriteRenderer = spriteRenderer;
                this.arrows = arrows;
                this.toggle = toggle;
            }
        };

        [SerializeField] bool validate;
        [SerializeField] List<SpriteRenderer> puddles;
        [SerializeField] List<Player> players;

        int PlayerToggleIsOnCount => players.Where(player => player.Toggle.isOn).Count();

        public void OnToggleValueChanged() {
            if (PlayerToggleIsOnCount < 3) {
                foreach (var player in players) {
                    player.Toggle.interactable = true;
                }
            }
            else {
                players.Select(player => player.Toggle).Where(toggle => !toggle.isOn).Select(toggle => {
                    return toggle.interactable = false;
                }).ToList();
            }
            var left = players.Take(4);
            var right = players.Skip(4);
            var leftIsOn = left.Where(player => player.Toggle.isOn);
            var rightIsOn = right.Where(player => player.Toggle.isOn);
            var leftIsOff = left.Where(player => !player.Toggle.isOn);
            var rightIsOff = right.Where(player => !player.Toggle.isOn);
            var leftCount = leftIsOn.Count();
            var rightCount = rightIsOn.Count();
            if (leftCount == 0) {
                left.First().Arrows.Select((arrow, index) => arrow.enabled = index == 3).ToList();
                left.Last().Arrows.Select((arrow, index) => arrow.enabled = index == 4).ToList();
                foreach (var player in left.Skip(1).Take(2)) {
                    player.Arrows.Select(arrow => arrow.enabled = false).ToList();
                }
            }
            if (rightCount == 0) {
                right.First().Arrows.Select((arrow, index) => arrow.enabled = index == 4).ToList();
                right.Last().Arrows.Select((arrow, index) => arrow.enabled = index == 5).ToList();
                foreach (var player in right.Skip(1).Take(2)) {
                    player.Arrows.Select(arrow => arrow.enabled = false).ToList();
                }
            }
            if (leftCount == 1) {
                foreach (var player in leftIsOn) {
                    player.Arrows.Select((arrow, index) => arrow.enabled = index == 0).ToList();
                }
                leftIsOff.First().Arrows.Select((arrow, index) => arrow.enabled = index == 3).ToList();
                foreach (var player in leftIsOff.Skip(1)) {
                    player.Arrows.Select(arrow => arrow.enabled = false).ToList();
                }
            }
            if (rightCount == 1) {
                foreach (var player in rightIsOn) {
                    player.Arrows.Select((arrow, index) => arrow.enabled = index == 2).ToList();
                }
                rightIsOff.Last().Arrows.Select((arrow, index) => arrow.enabled = index == 5).ToList();
                foreach (var player in rightIsOff.Take(2)) {
                    player.Arrows.Select(arrow => arrow.enabled = false).ToList();
                }
            }
            if (leftCount == 2) {
                leftIsOn.First().Arrows.Select((arrow, index) => arrow.enabled = index == 0).ToList();
                leftIsOn.Last().Arrows.Select((arrow, index) => arrow.enabled = index == 1).ToList();
                leftIsOff.First().Arrows.Select((arrow, index) => arrow.enabled = index == 3).ToList();
                leftIsOff.Last().Arrows.Select((arrow, index) => arrow.enabled = index == 4).ToList();
            }
            if (rightCount == 2) {
                rightIsOn.First().Arrows.Select((arrow, index) => arrow.enabled = index == 1).ToList();
                rightIsOn.Last().Arrows.Select((arrow, index) => arrow.enabled = index == 2).ToList();
                rightIsOff.First().Arrows.Select((arrow, index) => arrow.enabled = index == 4).ToList();
                rightIsOff.Last().Arrows.Select((arrow, index) => arrow.enabled = index == 5).ToList();
            }
            if (leftCount == 3) {
                leftIsOn.Select((player, playerIndex) => {
                    return player.Arrows.Select((arrow, arrowIndex) => {
                        return arrow.enabled = arrowIndex == (playerIndex switch {
                            0 => 0,
                            2 => 1,
                            _ => 2,
                        });
                    }).ToList();
                }).ToList();
                foreach (var player in leftIsOff) {
                    player.Arrows.Select((arrow, index) => arrow.enabled = index == 3).ToList();
                }
            }
            if (rightCount == 3) {
                rightIsOn.Select((player, playerIndex) => {
                    return player.Arrows.Select((arrow, arrowIndex) => {
                        return arrow.enabled = arrowIndex == (playerIndex switch {
                            0 => 1,
                            2 => 2,
                            _ => 0,
                        });
                    }).ToList();
                }).ToList();
                foreach (var player in rightIsOff) {
                    player.Arrows.Select((arrow, index) => arrow.enabled = index == 5).ToList();
                }
            }
        }

        void ValidatePuddles() => gameObject.transform
            .Find("Neurolink Puddles")
            .GetComponentsInChildren(true, puddles);
        void ValidatePlayers() {
            var spriteRenderers = new List<SpriteRenderer>();
            var allArrows = new List<LineRenderer>();
            var toggles = new List<Toggle>();
            gameObject.transform.Find("Players").GetComponentsInChildren(true, spriteRenderers);
            gameObject.transform.Find("Arrows").GetComponentsInChildren(true, allArrows);
            gameObject.transform.Find("Canvas/Toggles").GetComponentsInChildren(true, toggles);
            foreach (var toggle in toggles) {
#if UNITY_EDITOR
                var e = toggle.onValueChanged;
                while (0 < e.GetPersistentEventCount()) {
                    UnityEventTools.RemovePersistentListener(e, 0);
                }
                UnityEventTools.AddVoidPersistentListener(e, OnToggleValueChanged);
                e.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
#endif
            }
            players.Clear();
            spriteRenderers.Select((sprite, spriteIndex) => {
                var arrows = allArrows
                    .Where((arrows, index) => (index / 3) % 8 == spriteIndex)
                    .ToList();
                var toggle = toggles[spriteIndex];
                players.Add(new Player(sprite, arrows, toggle));
                return true;
            }).ToList();
        }
        void ValidatePlayerArrows() {
            foreach (var player in players) {
                foreach (var arrow in player.Arrows) {
                    var x = player.SpriteRenderer.transform.position.x;
                    var y = player.SpriteRenderer.transform.position.y;
                    var z = arrow.transform.position.z;
                    arrow.transform.position = new Vector3(x, y, z);
                }
            }
            void Straight(SpriteRenderer puddle, LineRenderer arrow) {
                var v = (Vector2)(puddle.transform.position - arrow.transform.position);
                arrow.positionCount = 2;
                arrow.SetPosition(0, new Vector3());
                arrow.SetPosition(1, v);
            }
            void Curve(SpriteRenderer puddle, LineRenderer arrow, float beginTh, float endTh) {
                var dx = puddle.transform.position.x - arrow.transform.position.x;
                var dy = puddle.transform.position.y - arrow.transform.position.y;
                var rx = Mathf.Abs(dx);
                var ry = Mathf.Abs(dy);
                arrow.positionCount = 16;
                arrow.SetPosition(0, new Vector3());
                arrow.SetPosition(15, new Vector3(dx, dy));
                foreach (var i in Enumerable.Range(1, 15)) {
                    var th = Mathf.Lerp(beginTh, endTh, (float)i / 16);
                    var x = rx * (Mathf.Cos(th) - Mathf.Cos(beginTh));
                    var y = ry * (Mathf.Sin(th) - Mathf.Sin(beginTh));
                    arrow.SetPosition(i, new Vector3(x, y));
                }
            }
            foreach (var player in players.Take(4)) {
                foreach (var arrow in player.Arrows.Where((arrow, index) => index % 3 == 0)) {
                    Curve(puddles[0], arrow, 1.5f * Mathf.PI, 1.0f * Mathf.PI);
                }
                foreach (var arrow in player.Arrows.Where((arrow, index) => index % 3 == 1)) {
                    Curve(puddles[1], arrow, 1.0f * Mathf.PI, 1.5f * Mathf.PI);
                }
                foreach (var arrow in player.Arrows.Where((arrow, index) => index % 3 == 2)) {
                    Straight(puddles[2], arrow);
                }
            }
            foreach (var player in players.Skip(4)) {
                foreach (var arrow in player.Arrows.Where((arrow, index) => index % 3 == 0)) {
                    Straight(puddles[0], arrow);
                }
                foreach (var arrow in player.Arrows.Where((arrow, index) => index % 3 == 1)) {
                    Curve(puddles[1], arrow, 2.0f * Mathf.PI, 1.5f * Mathf.PI);
                }
                foreach (var arrow in player.Arrows.Where((arrow, index) => index % 3 == 2)) {
                    Curve(puddles[2], arrow, 1.5f * Mathf.PI, 2.0f * Mathf.PI);
                }
            }
        }

        void OnValidate() {
            if (validate) {
                ValidatePuddles();
                ValidatePlayers();
                ValidatePlayerArrows();
            }
        }
    }
}
