using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 public enum EventType
{
    UPDATE_POS_GO,
    UPDATE_VEL_GO,
    CREATE_GO,
    DESTROY_GO,
    UPDATE_SCORE,
    HITPOINT,
    NONE
}
public class EventData
{
    public EventType EventType = EventType.NONE;
    public int ID = -1;
    public Vector3 trans;
}
