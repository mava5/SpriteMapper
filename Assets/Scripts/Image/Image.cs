
using UnityEngine;


public static class Image
{
    public static int canvasSize = 200;
    public static float[,] canvas = new float[canvasSize, canvasSize];

    public static void Update()
    {
        for (int x = 0; x < canvasSize; x++)
        {
            for (int y = 0; y < canvasSize; y++)
            {
                float value = canvas[x, y];

                if (value == 0) { continue; }

                Vector2 p1 = new Vector2(x - 0.5f, y - 0.5f) / canvasSize - new Vector2(0.5f, 0.5f);
                Vector2 p2 = new Vector2(x + 0.5f, y + 0.5f) / canvasSize - new Vector2(0.5f, 0.5f);
                Vector2 p3 = new Vector2(x - 0.5f, y + 0.5f) / canvasSize - new Vector2(0.5f, 0.5f);
                Vector2 p4 = new Vector2(x + 0.5f, y - 0.5f) / canvasSize - new Vector2(0.5f, 0.5f);

                p1 *= 4;
                p2 *= 4;
                p3 *= 4;
                p4 *= 4;

                Debug.DrawLine(p1, p2, Color.HSVToRGB((Time.time / 3f) % 1, 0.5f, value));
                Debug.DrawLine(p3, p4, Color.HSVToRGB((Time.time / 3f) % 1, 0.5f, value));
            }
        }
    }
}
