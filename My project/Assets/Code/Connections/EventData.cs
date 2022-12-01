using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 public enum EventType
{
    UPDATE,
    CREATE,
    DESTROY,
    NONE
}
public class EventData
{
    public EventType EventType = EventType.NONE;
    public int ID = -1;
    public Vector3 trans;
}
