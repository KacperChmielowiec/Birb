using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualSingle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MeshRenderer RenderVisualSingle;
    void Start()
    {
        
    }

    public void Active()
    {
        RenderVisualSingle.material.color = Color.green;
    }

    public void Light()
    {
        RenderVisualSingle.material.color = Color.gray;
    }

    public void Disabled()
    {
        RenderVisualSingle.material.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
