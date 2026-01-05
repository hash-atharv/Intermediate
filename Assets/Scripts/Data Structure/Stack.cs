using UnityEngine;

public class Stack :MonoBehaviour
{
    private  int[] data;
    private int top;
    private int max;
    


    private void Start()
    {
        max = 5;
        data = new int[5];
        top = -1;
    }

    public void push()
    {
        if (top == max - 1) { Debug.Log("Full"); }
        top++;
        


    }

    public void pop()
    {
        if ( top == -1) { Debug.Log("Empty"); }
        top--;
    }

}
