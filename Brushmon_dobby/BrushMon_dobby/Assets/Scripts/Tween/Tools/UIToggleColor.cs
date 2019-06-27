using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Toggle))]
public class UIToggleColor : MonoBehaviour
{
    public Graphic target;

    public Color activeColor;
    public Color deactiveColor;

    Toggle toggle;

    // Use this for initialization
    void Start()
    {
        toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnToggle);
            OnToggle(toggle.isOn);
        }
        else
        {
            LogManager.LogError("Toggle을 찾을수 없습니다.");
        }
    }

    public void OnToggle(bool state)
    {
        if (target != null)
        {
            target.color = state ? activeColor : deactiveColor;
        }
    }
}
