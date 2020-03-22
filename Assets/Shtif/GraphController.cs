using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GraphController : MonoBehaviour
{
    public SimulationController simulation;
    private RawImage _image;
    private Texture2D _texture;
    

    void Start()
    {
        _image = GetComponent<RawImage>();

        RectTransform rt = GetComponent<RectTransform>();

        _texture = new Texture2D(300, 100);
        _image.texture = _texture;

        Clear();
    }

    public void Clear()
    {
        Color bg = new Color32(20,20,20, 255);
        for (int y = 0; y < _texture.height; y++)
        {
            for (int x = 0; x < _image.texture.width; x++)
            {
                _texture.SetPixel(x, y, bg);
            }
        }
        _texture.Apply();
    }

    void Update()
    {
        if (!simulation.IsRunning()) return;
        
        int count = simulation.infected;
        int count2 = simulation.quarantined;
        for (int i = 1; i < count+count2; ++i)
        {
            _texture.SetPixel(simulation.time*3, i, i<count2 ? Color.magenta : Color.red);
            _texture.SetPixel(simulation.time*3+1, i, i<count2 ? Color.magenta : Color.red);
            _texture.SetPixel(simulation.time*3+2, i, i<count2 ? Color.magenta : Color.red);
        }            
        
        count = simulation.cured;
        count2 = simulation.dead;
        for (int i = 1; i < count+count2; ++i)
        {
            _texture.SetPixel(simulation.time*3, _texture.height-i-1, i<count2 ? Color.blue : Color.green);
            _texture.SetPixel(simulation.time*3+1, _texture.height-i-1, i<count2 ? Color.blue : Color.green);
            _texture.SetPixel(simulation.time*3+2, _texture.height-i-1, i<count2 ? Color.blue : Color.green);
        }
        
        _texture.Apply();
    }
}
