using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GabrielBertasso.UI
{
    public static class EventManager
    {
        private static EventSystem s_eventSystem;
        private static GameObject s_actualSelectedObject;

        public static EventSystem EventSystem
        {
            get
            {
                if (s_eventSystem == null)
                {
                    s_eventSystem = EventSystem.current;
                }

                return s_eventSystem;
            }
        }

        public static GameObject CurrentSelectedObject
        {
            get => EventSystem != null ? EventSystem.currentSelectedGameObject : null;
            set
            {
                if (EventSystem != null)
                {
                    EventSystem.SetSelectedGameObject(value);
                    ActualSelectedObject = value;
                }
            }
        }

        public static GameObject ActualSelectedObject
        {
            get => s_actualSelectedObject;
            set
            {
                if (s_actualSelectedObject != value)
                {
                    s_actualSelectedObject = value;
                    ActualSelectedObjectChanged?.Invoke(s_actualSelectedObject);
                }
            }
        }

        public static Action<GameObject> ActualSelectedObjectChanged;
    }
}