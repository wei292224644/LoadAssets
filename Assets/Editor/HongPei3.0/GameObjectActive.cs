using UnityEngine;

public class GameObjectActive : MonoBehaviour
{
    public GameObject[] gos_Hide;
    public GameObject[] gos_Show;

    [HideInInspector]
    public bool bl_IsBoom = false;
}
