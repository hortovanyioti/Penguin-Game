using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SensitivitySlider : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Slider m_Slider;
    private PlayerScript m_Player;
    void Start()
    {
        m_Slider = GetComponent<Slider>();
        m_Player = GameManagerScript.instance.PlayerScripts[0];
        m_Slider.value = m_Player.LookSensitivity;//TODO: Multiplayer
    }
    public void OnBeginDrag(PointerEventData eventData)
    {

    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        GameManagerScript.instance.PlayerScripts[0].LookSensitivity = m_Slider.value; //TODO: Multiplayer
    }
    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
