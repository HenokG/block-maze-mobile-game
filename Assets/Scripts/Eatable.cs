using UnityEngine;

public class Eatable : MonoBehaviour
{
    public Color EatenColor;
    public Color DefaultColor;
    public MyEventManager MyEventManger;

    public bool IsEaten;
    
    // Use this for initialization
    void Start()
    {
        MyEventManger = FindObjectOfType<MyEventManager>();
        IsEaten = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        IsEaten = true;
        GetComponent<Renderer>().material.color = EatenColor;
        if (MyEventManger)
        {
            MyEventManger.OnEatingUpdate();
        }
    }

    void OnTriggerExit(Collider other)
    {
        IsEaten = false;
        GetComponent<Renderer>().material.color = DefaultColor;
        MyEventManger.OnEatingUpdate();
    }
}