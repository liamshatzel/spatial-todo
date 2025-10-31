using System;
using System.Collections.Generic;
using UnityEngine;

public class TodoManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    public List<Transform> todoItems;
    [SerializeField]
    public List<Boolean> occupied;
    void Start()
    {
        todoItems = new List<Transform>();
        occupied = new List<Boolean>();

        for (int i = 0; i < todoItems.Count; i++)
        {
            occupied.Add(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (todoItems.Count > 0)
        {
            for (int i = 0; i < todoItems.Count; i++)
            {
                if (todoItems[i] == null)
                {
                    todoItems.RemoveAt(i);
                    occupied.RemoveAt(i);
                }
                else
                {
                    occupied[i] = true;
                }
            }
        }
    }

    public Transform GetAvailableIndex()
    {
        for (int i = 0; i < occupied.Count; i++)
        {
            if (!occupied[i])
            {
                return todoItems[i];
            }
        }
        return null; // No available index
    }
}
