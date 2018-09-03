using UnityEngine;
using UnityEngine.EventSystems;

namespace nm
{
    public class TooltipTextUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public string text;
        public bool arrayShow = false;
        //public bool active = true;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData e)
        {
            Tooltip.text = text;
            Tooltip.arrayShow = arrayShow;
            //Tooltip.active = active;
            Tooltip.isUI = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData e)
        {
            Tooltip.isUI = false;
        }
    }
}