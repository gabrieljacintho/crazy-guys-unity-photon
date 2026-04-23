using System;
using GabrielBertasso.Helpers;
using UnityEditor;

namespace GabrielBertasso.AtomsAddon.Editor
{
    public static class AtomMenus
    {
        [MenuItem("Tools/Unity Atoms/Update All Atoms Spreadsheets")]
        public static void UpdateAll()
        {
            AtomSpreadsheet[] atomSpreadsheets = ResourcesHelper.LoadAll<AtomSpreadsheet>();
            foreach (AtomSpreadsheet atomSpreadsheet in atomSpreadsheets)
            {
                try
                {
                    atomSpreadsheet.Update(false);
                }
                catch (Exception error)
                {
                    Debug.LogError("Atoms spreadsheet \"" + atomSpreadsheet.name + "\" update failed! " + error.Message);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("All atom spreadsheets updated successfully. (" + atomSpreadsheets.Length + ")");
        }
    }
}