using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace ProceduralCity
{
    public class BuildingFunctionCollapse : MonoBehaviour
    {
        [SerializeField] private int size;
        [SerializeField] private TileR[] tileObjects;
        [SerializeField] private CellR cellPrefab;
        [SerializeField] private TileR[] buildingBackupTile;
        [SerializeField, Range(0.7f, 1f)] private float buildingDensity = 0.8f;

        private List<CellR> cellGrids;
        private Dictionary<TileR, int> tileIndexCache;

        private GameObject generatedCityPrefab;

        private void Awake()
        {

            tileIndexCache = new Dictionary<TileR, int>(tileObjects.Length);
            for (int i = 0; i < tileObjects.Length; i++)
                tileIndexCache[tileObjects[i]] = i;

            cellGrids = new List<CellR>(size * size);
            InitializeGrid();
        }

        [ContextMenu("Initialize City")]
        private void InitializeGrid()
        {
            generatedCityPrefab = new GameObject("Generated City");

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    CellR newCell = Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.identity);
                    newCell.transform.parent = generatedCityPrefab.transform;
                    newCell.CreateCell(false, tileObjects);
                    cellGrids.Add(newCell);
                }
            }

            StartCoroutine(EntropyLoop());
        }

        IEnumerator EntropyLoop()
        {
            while (true)
            {
                List<CellR> tempGrid = cellGrids.Where(c => !c.collapsed).ToList();
                if (tempGrid.Count == 0) yield break;

                int minEntropy = tempGrid.Min(c => c.tileOptions != null ? c.tileOptions.Length : 0);
                tempGrid.RemoveAll(c => (c.tileOptions == null ? 0 : c.tileOptions.Length) != minEntropy);

                CollapseCell(tempGrid);
                yield return null;
            }
        }

        private void CollapseCell(List<CellR> candidates)
        {
            CellR cellToCollapse = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            cellToCollapse.collapsed = true;

            if (cellToCollapse.tileOptions == null || cellToCollapse.tileOptions.Length == 0)
                cellToCollapse.tileOptions = new TileR[] { buildingBackupTile[UnityEngine.Random.Range(0, buildingBackupTile.Length)] };

            TileR selectedTile;
            if (UnityEngine.Random.value <= buildingDensity)
            {
                var buildingTiles = cellToCollapse.tileOptions.Where(t => t != null && t.isBuilding).ToArray();
                selectedTile = SafePick(buildingTiles, SafePick(cellToCollapse.tileOptions,
                    buildingBackupTile[UnityEngine.Random.Range(0, buildingBackupTile.Length)]));
            }
            else
            {
                selectedTile = SafePick(cellToCollapse.tileOptions,
                    buildingBackupTile[UnityEngine.Random.Range(0, buildingBackupTile.Length)]);
            }

            cellToCollapse.tileOptions = new TileR[] { selectedTile };
            TileR newTile = Instantiate(selectedTile, cellToCollapse.transform.position, selectedTile.transform.rotation);

            newTile.transform.parent = generatedCityPrefab.transform;

            UpdateNeighbors();
        }

        private void UpdateNeighbors()
        {
            var newGeneration = new List<CellR>(cellGrids);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int index = x + y * size;
                    if (cellGrids[index].collapsed) continue;

                    List<TileR> options = new List<TileR>(tileObjects);

                    if (y > 0) FilterOptions(options, cellGrids[x + (y - 1) * size], dir: 0);
                    if (x < size - 1) FilterOptions(options, cellGrids[x + 1 + y * size], dir: 1);
                    if (y < size - 1) FilterOptions(options, cellGrids[x + (y + 1) * size], dir: 2);
                    if (x > 0) FilterOptions(options, cellGrids[x - 1 + y * size], dir: 3);

                    if (options.Count == 0) options.Add(buildingBackupTile[UnityEngine.Random.Range(0, buildingBackupTile.Length)]);

                    newGeneration[index].ReCreateCell(options.ToArray());
                }
            }

            cellGrids = newGeneration;
        }

        private void FilterOptions(List<TileR> options, CellR neighbor, int dir)
        {
            if (neighbor == null || neighbor.tileOptions == null || neighbor.tileOptions.Length == 0) return;

            HashSet<TileR> validSet = new HashSet<TileR>();

            foreach (TileR possibleOption in neighbor.tileOptions)
            {
                if (possibleOption == null || !tileIndexCache.TryGetValue(possibleOption, out int tileIndex))
                    continue;

                TileR[] valid;
                if (dir == 0) valid = tileObjects[tileIndex].down;
                else if (dir == 1) valid = tileObjects[tileIndex].right;
                else if (dir == 2) valid = tileObjects[tileIndex].up;
                else valid = tileObjects[tileIndex].left;

                if (valid != null)
                {
                    for (int i = 0; i < valid.Length; i++)
                        if (valid[i] != null) validSet.Add(valid[i]);
                }
            }

            for (int i = options.Count - 1; i >= 0; i--)
            {
                if (!validSet.Contains(options[i]))
                    options.RemoveAt(i);
            }
        }

        private TileR SafePick(TileR[] arr, TileR fallback)
        {
            if (arr == null || arr.Length == 0) return fallback;
            int r = UnityEngine.Random.Range(0, arr.Length);
            return arr[r] != null ? arr[r] : fallback;
        }
    }
}