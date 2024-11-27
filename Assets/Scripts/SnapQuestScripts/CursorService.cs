using UnityEngine;

//IF YOU ARE HAVING ISSUES WITH CURSOR, ITS LIKELY PLAY MAKER GUI "ControlMouseCursor" is enabled 
public class CursorService
{
    public CursorService()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
