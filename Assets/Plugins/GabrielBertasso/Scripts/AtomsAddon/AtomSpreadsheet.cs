using System;
using System.Collections.Generic;
using GabrielBertasso.Helpers;
using Sirenix.OdinInspector;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace GabrielBertasso.AtomsAddon
{
    [Serializable]
    public struct AtomSpreadsheetCell
    {
        public AtomBaseVariable AtomVariable;
        public string ColumnId;
        public string RowId;
    }

    [CreateAssetMenu(menuName = "Unity Atoms/Spreadsheet", fileName = "AtomSpreadsheet")]
    public class AtomSpreadsheet : ScriptableObject
    {
        [Sirenix.OdinInspector.FilePath(ParentFolder = "Assets", Extensions = "csv", RequireExistingPath = true)]
        public string Path;
        public List<AtomSpreadsheetCell> AtomCells;
        public bool OverrideColumnIds;
        [ShowIf("OverrideColumnIds")]
        public string ColumnId;
        public bool OverrideRowIds;
        [ShowIf("OverrideRowIds")]
        public string RowId;


        [Button]
        public void Update()
        {
            Update(true);
        }

        public void Update(bool debug)
        {
            string path = Application.dataPath + "/" + Path;

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Invalid path! (\"" + path + "\")", this);
                return;
            }

            if (AtomCells == null || AtomCells.Count == 0)
            {
                return;
            }

            string[,] cells = CSVHelper.ConvertToArray(path);
            if (cells == null)
            {
                return;
            }

            if (OverrideColumnIds)
            {
                OverrideColumnIdsFunc();
            }

            if (OverrideRowIds)
            {
                OverrideRowIdsFunc();
            }

            foreach (AtomSpreadsheetCell atomCell in AtomCells)
            {
                UpdateAtomCell(cells, atomCell);
#if UNITY_EDITOR
                EditorUtility.SetDirty(atomCell.AtomVariable);
#endif
            }

            if (debug)
            {
#if UNITY_EDITOR
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
                Debug.Log("Atoms spreadsheet updated successfully. (\"" + name + "\")", this);
            }
        }

        private void UpdateAtomCell(string[,] cells, AtomSpreadsheetCell atomCell)
        {
            if (atomCell.AtomVariable == null)
            {
                return;
            }

            if (!CSVHelper.FindCellWithIds(cells, atomCell.ColumnId, atomCell.RowId, out string cell))
            {
                Debug.LogWarning("Cell not found! (\"" + atomCell.ColumnId + "\", \"" + atomCell.RowId + "\")", this);
                SetAtomVariableToDefault(atomCell.AtomVariable);
                return;
            }

            if (string.IsNullOrEmpty(cell))
            {
                Debug.LogWarning("Cell is empty! (\"" + atomCell.ColumnId + "\", \"" + atomCell.RowId + "\")", this);
                SetAtomVariableToDefault(atomCell.AtomVariable);
                return;
            }

            switch (atomCell.AtomVariable)
            {
                case StringVariable stringVariable:
                    stringVariable.InitialValue = cell;
                    stringVariable.Value = cell;
                    break;

                case FloatVariable floatVariable:
                    if (!float.TryParse(cell, out float @float))
                    {
                        Debug.LogError("Could not convert cell to float! (\""
                                       + atomCell.ColumnId + "\", \"" + atomCell.RowId + "\")", this);
                    }
                    floatVariable.InitialValue = @float;
                    floatVariable.Value = @float;
                    break;

                case IntVariable intVariable:
                    if (!int.TryParse(cell, out int @int))
                    {
                        Debug.LogError("Could not convert cell to int! (\""
                                       + atomCell.ColumnId + "\", \"" + atomCell.RowId + "\")", this);
                    }
                    intVariable.InitialValue = @int;
                    intVariable.Value = @int;
                    break;

                case DoubleVariable doubleVariable:
                    if (!double.TryParse(cell, out double @double))
                    {
                        Debug.LogError("Could not convert cell to double! (\""
                                       + atomCell.ColumnId + "\", \"" + atomCell.RowId + "\")", this);
                    }
                    doubleVariable.InitialValue = @double;
                    doubleVariable.Value = @double;
                    break;

                default:
                    Debug.LogError("Type not supported! (" + atomCell.AtomVariable.GetType().Name + ")", atomCell.AtomVariable);
                    SetAtomVariableToDefault(atomCell.AtomVariable);
                    break;
            }
        }

        private void SetAtomVariableToDefault(AtomBaseVariable atomVariable)
        {
            switch (atomVariable)
            {
                case StringVariable stringVariable:
                    stringVariable.InitialValue = default;
                    stringVariable.Value = default;
                    break;

                case FloatVariable floatVariable:
                    floatVariable.InitialValue = default;
                    floatVariable.Value = default;
                    break;

                case IntVariable intVariable:
                    intVariable.InitialValue = default;
                    intVariable.Value = default;
                    break;

                case DoubleVariable doubleVariable:
                    doubleVariable.InitialValue = default;
                    doubleVariable.Value = default;
                    break;

                default:
                    Debug.LogError("Type not supported! (" + atomVariable.GetType().Name + ")", atomVariable);
                    atomVariable.BaseValue = null;
                    break;
            }
        }

        private void OverrideColumnIdsFunc()
        {
            if (AtomCells == null)
            {
                return;
            }

            for (int i = 0; i < AtomCells.Count; i++)
            {
                AtomSpreadsheetCell atomCell = AtomCells[i];
                atomCell.ColumnId = ColumnId;
                AtomCells[i] = atomCell;
            }
        }

        private void OverrideRowIdsFunc()
        {
            if (AtomCells == null)
            {
                return;
            }

            for (int i = 0; i < AtomCells.Count; i++)
            {
                AtomSpreadsheetCell atomCell = AtomCells[i];
                atomCell.RowId = RowId;
                AtomCells[i] = atomCell;
            }
        }
    }
}