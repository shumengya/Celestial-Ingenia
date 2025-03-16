using UnityEngine;
using UnityEngine.EventSystems;

public class ClickResponder : MonoBehaviour, IPointerClickHandler
{
    public delegate void ClickEvent();
    public event ClickEvent OnClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClicked != null)
        {
            OnClicked();
        }
    }
}