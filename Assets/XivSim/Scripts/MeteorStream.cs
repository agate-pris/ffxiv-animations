
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MeteorStream : MonoBehaviour, ITimeControl, IPropertyPreview {
    [SerializeField] GameObject target = default;

    public void OnControlTimeStart() {
        if (!target) {
            return;
        }
        transform.position = target.transform.position;
    }
    public void OnControlTimeStop() { }
    public void SetTime(double _time) { }
    public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        => driver.AddFromName<Transform>(transform.gameObject, "m_LocalPosition");
}
