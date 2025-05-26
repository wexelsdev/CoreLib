using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.DevTools.Matrix
{
    public static class MatrixPlacer
    {
        public static void PlaceInMatrix(this List<GameObject> objects, Vector3 center, float spacing)
        {
            int count = objects.Count;
            int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
            int rows = Mathf.CeilToInt((float)count / columns);

            Vector3 start = center - new Vector3((columns - 1) * spacing / 2f, 0f, (rows - 1) * spacing / 2f);

            for (int i = 0; i < objects.Count; i++)
            {
                int row = i / columns;
                int column = i % columns;

                Vector3 position = start + new Vector3(column * spacing, 0f, row * spacing);
                objects[i].transform.position = position;
            }
        }
    }
}