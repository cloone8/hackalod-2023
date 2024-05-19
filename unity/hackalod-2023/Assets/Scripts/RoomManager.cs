using UnityEngine;
using UnityEngine.Assertions;

public class RoomManager : MonoBehaviour
{
    public int numExits = -1;
    public Entrance entrance;

    public void Start()
    {
        Assert.IsTrue(numExits >= 0);
    }
}
