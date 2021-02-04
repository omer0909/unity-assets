using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mazeTexturePrim : MonoBehaviour
{
    public int matrix = 29;
    private bool[,] mazeWall;
    private float[,] randomWals;
    private float wallValue = 1;
    void Start()
    {
        mazeWall = new bool[matrix, matrix];
        randomWals = new float[matrix / 2, matrix / 2];

        for (int x = 0; x < matrix / 2; x++)
        {
            for (int y = 0; y < matrix / 2; y++)
            {
                randomWals[x, y] = Random.value;
            }
        }

        List<Vector2Int> look = new List<Vector2Int>();
        look.Add(Vector2Int.one);
        mazeWall[1, 1] = true;

        Texture2D texture = new Texture2D(matrix, matrix);
        texture.filterMode = FilterMode.Point;

        while (true)
        {
            float min = 1;
            Vector2Int pos = Vector2Int.zero;
            Vector2Int wallPos = Vector2Int.zero;
            for (int i = 0; i < look.Count; i++)
            {
                Vector2Int direction = control(look[i]);

                if (direction == Vector2Int.zero)
                {
                    look.RemoveAt(i);
                }
                else
                {
                    if (wallValue < min)
                    {
                        min = wallValue;
                        pos = look[i] + direction * 2;
                        wallPos = look[i] + direction;
                    }
                }
            }


            if (pos != Vector2Int.zero)
            {
                mazeWall[pos.x, pos.y] = true;
                mazeWall[wallPos.x, wallPos.y] = true;
                look.Add(pos);
            }

            if (look.Count == 0)
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
        wallValue = 1;
        if (direction.Count != 0)
        {
            int index = 0;
            float min = 1;
            for (int i = 0; i < direction.Count; i++)
            {
                wallValue = randomWals[(pos.x + direction[i].x * 2) / 2, (pos.y + direction[i].y * 2) / 2];
                if (wallValue < min)
                {
                    min = wallValue;
                    index = i;
                }
            }
            result = direction[index];
        }
        return result;
    }
}