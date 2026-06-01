using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralCity
{
    public class CellR : MonoBehaviour
    {
        public bool collapsed;

        public TileR[] tileOptions;

        public void CreateCell(bool collapesedState, TileR[] tiles)
        {
            collapsed = collapesedState;
            tileOptions = tiles;
        }

        public void ReCreateCell(TileR[] tiles)
        {
            tileOptions = tiles;
        }
    }
}
