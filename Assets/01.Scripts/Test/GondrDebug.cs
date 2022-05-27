using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gondr
{
    public class GondrDebug
    {
        private static RectTransform _canvasTrm;
        static GondrDebug()
        {
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if(canvas == null)
            {
                Debug.LogError("UI 캔버스가 존재하지 않습니다. 디버그모드 사용불가");
            }
            else
            {
                _canvasTrm = canvas.GetComponent<RectTransform>();
            }
        }

        public static void CreateButton(Vector2 pos, string name, Vector2 size, Action action)
        {
            GameObject button = new GameObject($"Btn_{name}");
            RectTransform rect = button.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            button.AddComponent<Image>();
            CustomButton btnCompo = button.AddComponent<CustomButton>();
            btnCompo.BtnAction = action;

            GameObject text = new GameObject("ButtonText");
            RectTransform textRect = text.AddComponent<RectTransform>();
            textRect.SetParent(rect);
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;

            Text textCompo = text.AddComponent<Text>();
            textCompo.text = name;
            textCompo.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            textCompo.color = Color.black;
            textCompo.resizeTextForBestFit = true;

            

            rect.SetParent( _canvasTrm);
        }
    }

    public class CustomButton : MonoBehaviour, IPointerClickHandler
    {
        private Action _btnAction = null;
        public Action BtnAction { set => _btnAction = value; }
        public void OnPointerClick(PointerEventData eventData)
        {
            _btnAction?.Invoke();
        }

    }

}

