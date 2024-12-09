using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool IsOccupied;

    public void ToggleOccupied()
    {
        IsOccupied = !IsOccupied;
    }
}