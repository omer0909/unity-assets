using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mazeTexture : MonoBehaviour
{
    public int matrix = 32;
    bool[,] mazeWall;
    void Start()
    {
        mazeWall = new bool[matrix, matrix];

        Texture2D texture = new Texture2D(matrix, matrix);
        texture.filterMode = FilterMode.Point;
        List<Vector2Int> way = new List<Vector2Int>();

        way.Add(Vector2Int.zero);
        mazeWall[0, 0] = true;

        int index = 0;

        while (true)
        {
            Vector2Int direction = control(way[index]);

            if (direction == Vector2Int.zero)
            {
                index--;
                way.RemoveAt(way.Count - 1);
            }
            else
            {
                way.Add(way[index] + direction * 2);

                mazeWall[(way[index] + direction).x, (way[index] + direction).y] = true;
                mazeWall[(way[index] + direction * 2).x, (way[index] + direction * 2).y] = true;
                index++;
            }

            if (index == 0)
            {
                break;
            }
        }

        for (int x = 0; x < matrix; x++)
        {
            for (int y = 0; y < matrix; y++)
            {
                if (mazeWall[x, y] == false)
                {
                    texture.SetPixel(x, y, Color.black);
                }
                else
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }
        }
        texture.Apply();
        GetComponent<MeshRenderer>().material.mainTexture = texture;
    }
    Vector2Int control(Vector2Int pos)
    {
        List<Vector2Int> direction = new List<Vector2Int>();
        if (pos.x + 2 < matrix)
        {
            if (!mazeWall[pos.x + 2, pos.y])
            {

                direction.Add(Vector2Int.right);
            }
        }

        if (pos.y + 2 < matrix)
        {
            if (!mazeWall[pos.x, pos.y + 2])
            {
                direction.Add(Vector2Int.up);
            }
        }

        if (pos.x - 1 > 0)
        {
            if (!mazeWall[pos.x - 2, pos.y])
            {
                direction.Add(Vector2Int.left);
            }
        }

        if (pos.y - 1 > 0)
        {
            if (!mazeWall[pos.x, pos.y - 2])
            {
                direction.Add(Vector2Int.down);
            }
        }
        Vector2Int result = Vector2Int.zero;
        if (direction.Count != 0)
        {
            result = direction[(int)Random.Range(0, direction.Count)];
        }
        return result;
    }
}
