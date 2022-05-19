
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class AreaOfEffect : MonoBehaviour, ITimeControl, IPropertyPreview {
    [SerializeField] GameObject target = default;
    [SerializeField] AnimationCurve alphaAnimationCurve = AnimationCurve.Linear(0, 2.0f / 3, 1, 0);
    [SerializeField] SpriteRenderer spriteRenderer = default;
    [SerializeField] LineRenderer lineRenderer = default;
    public GameObject Target {
        get => target;
        set { target = value; }
    }

    void SetAlpha(float alpha) {
        if (!spriteRenderer) {
            return;
        }
        var color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
    void UpdateAlpha(float time) {
        var alpha = alphaAnimationCurve.Evaluate(time);
        SetAlpha(alpha);
    }
    void SetAlphaKeys(float alpha) {
        if (!lineRenderer) {
            return;
        }
        var colorGradient = lineRenderer.colorGradient;
        var alphaKeys = colorGradient.alphaKeys;
        for (var i = 0; i < alphaKeys.Length; i++) {
            alphaKeys[i] = new GradientAlphaKey(alpha, alphaKeys[i].time);
        }
        colorGradient.alphaKeys = alphaKeys;
        lineRenderer.colorGradient = colorGradient;
    }
    void UpdateAlphaKeys(float time) {
        var alpha = alphaAnimationCurve.Evaluate(time);
        SetAlphaKeys(alpha);
    }
    public void SetTime(double time) {
        var ftime = (float)time;
        UpdateAlpha(ftime);
        UpdateAlphaKeys(ftime);
    }
    public void OnControlTimeStart() {
        if (target) {
            transform.position = target.transform.position;
        }
        SetAlpha(0);
        SetAlphaKeys(0);
    }
    public void OnControlTimeStop() {
        SetAlpha(0);
        SetAlphaKeys(0);
    }
    public void GatherProperties(PlayableDirector director, IPropertyCollector driver) {
#if UNITY_EDITOR
        if (target) {
            driver.AddFromName<Transform>(transform.gameObject, "m_LocalPosition");
        }
        if (spriteRenderer) {
            driver.AddFromName<SpriteRenderer>(spriteRenderer.gameObject, "m_Color.a");
        }
        if (lineRenderer) {
            driver.AddFromName<LineRenderer>(lineRenderer.gameObject, "m_Parameters.colorGradient");
        }
#endif
    }
    void OnValidate() {
        if (!spriteRenderer) {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (!spriteRenderer) {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (!lineRenderer) {
            lineRenderer = GetComponent<LineRenderer>();
        }
        if (!lineRenderer) {
            lineRenderer = GetComponentInChildren<LineRenderer>();
        }
    }
}
