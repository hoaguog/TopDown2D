using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public Texture2D fogOfWarTexture; 
    public SpriteMask spriteMask;     
    public FieldOfView fieldOfView;  

    private Vector2 worldScale;      
    private Vector2Int pixelScale;  

    public void Awake()
    {
        pixelScale.x = fogOfWarTexture.width;
        pixelScale.y = fogOfWarTexture.height;

        worldScale.x = pixelScale.x / 100f * transform.localScale.x;
        worldScale.y = pixelScale.y / 100f * transform.localScale.y;


        for (int i = 0; i < pixelScale.x; i++)
        {
            for (int j = 0; j < pixelScale.y; j++)
            {
                fogOfWarTexture.SetPixel(i, j, Color.black);
            }
        }
        fogOfWarTexture.Apply();
        CreateSprite();
    }

    private Vector2Int WorldToPixel(Vector2 position)
    {
        Vector2Int pixelPosition = Vector2Int.zero;

        float dx = position.x - transform.position.x;
        float dy = position.y - transform.position.y;

        pixelPosition.x = Mathf.RoundToInt(.5f * pixelScale.x + dx * (pixelScale.x / worldScale.x));
        pixelPosition.y = Mathf.RoundToInt(.5f * pixelScale.y + dy * (pixelScale.y / worldScale.y));
        return pixelPosition;
    }

    public void MakeHole(Vector2 position, float holeRadius)
    {
        Vector2Int pixelPosition = WorldToPixel(position);
        int radius = Mathf.RoundToInt(holeRadius * pixelScale.x / worldScale.x);
        int px, nx, py, ny, distance;

        for (int i = 0; i < radius; i++)
        {
            distance = Mathf.RoundToInt(Mathf.Sqrt(radius * radius - i * i));
            for (int j = 0; j < distance; j++)
            {
                px = Mathf.Clamp(pixelPosition.x + i, 0, pixelScale.x - 1);
                nx = Mathf.Clamp(pixelPosition.x - i, 0, pixelScale.x - 1);
                py = Mathf.Clamp(pixelPosition.y + j, 0, pixelScale.y - 1);
                ny = Mathf.Clamp(pixelPosition.y - j, 0, pixelScale.y - 1);

                fogOfWarTexture.SetPixel(px, py, Color.clear);
                fogOfWarTexture.SetPixel(nx, py, Color.clear);
                fogOfWarTexture.SetPixel(px, ny, Color.clear);
                fogOfWarTexture.SetPixel(nx, ny, Color.clear);
            }
        }
        fogOfWarTexture.Apply();
        CreateSprite();
    }

    private void CreateSprite()
    {
        spriteMask.sprite = Sprite.Create(fogOfWarTexture, new Rect(0, 0, fogOfWarTexture.width, fogOfWarTexture.height), Vector2.one * .5f, 100);
    }

    private void Update()
    {

        if (fieldOfView != null)
        {
            foreach (Transform target in fieldOfView.visibleTargets)
            {
                MakeHole(target.position, fieldOfView.viewRadius / 10f); 
            }
        }
    }
}
