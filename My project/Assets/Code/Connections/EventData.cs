using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 public enum EventType
{
    UPDATE_POS_GO,
    CREATE_POWERUP,
    UPDATE_VEL_GO,
    DESTROY_POWERUP,
    UPDATE_SCORE,
    HITPOINT,
    UPDATE_POWERUP,
    NONE
}
public class EventData
{
    public EventType EventType = EventType.NONE;
    public int ID = -1;
    public Vector3 trans;
}
