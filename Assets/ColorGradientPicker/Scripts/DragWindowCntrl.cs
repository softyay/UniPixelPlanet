using System;
using System.Collections.Generic;
using UnityEngine;

// Fully rewritten
[RequireComponent(typeof(RectTransform))]
public class DragWindowCntrl : MonoBehaviour
{
    public Action<bool> DragEvent;

    private static List< Action<bool> > dragEvents = new();

    private RectTransform window;

    private Vector3 windowOffsetFromMouse;

    private Vector3 mousePosScreenSpace
    {
        get
        {
            return Input.mousePosition;
        }
    }

    private Vector3 windowPosScreenSpace
    {
        get
        {
            return Camera.main.WorldToScreenPoint(window.position);
        }
    }


    private void Awake()
    {
        window = (RectTransform)transform;
    }

    public void BeginDrag()
    {
        windowOffsetFromMouse = mousePosScreenSpace - windowPosScreenSpace;

        PerformActions(true);
    }

    public void Drag()
    {
        Vector3 newPosScreenSpace = mousePosScreenSpace - windowOffsetFromMouse;

        Vector3 newPosWorldSpace = Camera.main.ScreenToWorldPoint(newPosScreenSpace);

        window.SetPositionAndRotation(newPosWorldSpace, window.rotation);
    }

    public void EndDrag()
    {
        PerformActions(false);
    }

    public void Subscribe(Action<bool> onDrag)
    {
        dragEvents.Add(onDrag);
    }

    public void Unsubscribe(Action<bool> onDrag)
    {
        if (!dragEvents.Contains(onDrag)) { return; }

        dragEvents.Remove(onDrag);
    }

    private void PerformActions(bool dragIsBeginning)
    {
        List<int> badIndices = new();

        for (int i = 0; i < dragEvents.Count; i++)
        {
            if (dragEvents[i] == null)
            {
                badIndices.Add(i);
                continue;
            }

            dragEvents[i].Invoke(dragIsBeginning);
        }

        CleanUp(badIndices);
    }

    private void CleanUp(List<int> badIndices)
    {
        foreach (int index in badIndices)
        {
            dragEvents.RemoveAt(index);
        }
    }
}
