using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StoryGenerator
{

    public static void Generate(PuzzleGenerator.Power power)
    {
        
        string arcFileStructure = GetArcFileStructure();
        string puzzleFileStructure = GetPuzzleFileStructure();
        string puzzleMetaFileStructure = GetPuzzleMetaFileStructure();
        string arcGuidLine = "  - { fileID: 11400000, guid: %guid%, type: 2}\n";
        string arcGuidLines = "";
        string[] powerGuids = {
            "{fileID: 0}", 
            "{fileID: 11400000, guid: 4101cfe47cbd4904fb8012a0a28e3c81, type: 2}",
            "{fileID: 11400000, guid: 69854775bd1bed147ae2f0fe8f8a469a, type: 2}",
            "{fileID: 11400000, guid: 22f861e77500e3d4b9efaf9bf4f69a4f, type: 2}" };
        for (int i = 0; i<=12; i++)
        {
            float difficulty = 1 + i / 2f;
            PuzzleDifficultySetter pds = new PuzzleDifficultySetter(power, difficulty);
            PuzzleGenerator generator = new PuzzleGenerator();
            pds.ApplyTo(generator);
            GeneratorGridData ggd;
            do
            {
                ggd = generator.Generate();
            } while (ggd == null);
            string gridPrefillText = InputFieldUpdater.GetPrefillGridText(ggd);
            string gridNextText = InputFieldUpdater.GetNextText(ggd);
            string guid = "22f861e77500e3d4b9efaf9bf4f69";
            string id = ""+(int)power;
            if (i < 10) id += "0" + i;
            else id += i;
            guid += id;
            gridPrefillText = gridPrefillText.Replace("\n", "\n\n    ");
            gridNextText = gridNextText.Replace("\n", "\n\n    ");
            arcGuidLines += arcGuidLine.Replace("%guid%", guid);
            string puzzleFile = puzzleFileStructure.Replace("%prefill%", gridPrefillText);
            puzzleFile = puzzleFile.Replace("%next%", gridNextText);
            puzzleFile = puzzleFile.Replace("%powerAtStart%", powerGuids[(int)power]);
            string puzzleMetaFile = puzzleMetaFileStructure.Replace("%guid%", guid);
            File.WriteAllText("GeneratedPuzzles2/puzzle" + id + ".asset", puzzleFile);
            File.WriteAllText("GeneratedPuzzles2/puzzle" + id + ".asset.meta", puzzleMetaFile);

        }
        string arcFile = arcFileStructure.Replace("%guidLines%", arcGuidLines);
        File.WriteAllText($"GeneratedPuzzles2/arc{(int)power}.asset", arcFile);

    }

    private static string GetPuzzleMetaFileStructure()
    {
        return File.ReadAllText("puzzleMetaStructure.txt");
    }

    private static string GetPuzzleFileStructure()
    {
        return File.ReadAllText("puzzleStructure.txt");
    }

    private static string GetArcFileStructure()
    {
        return File.ReadAllText("arcStructure.txt");
    }
}
