using System.IO;
using UnityEngine;
using static PuzzleGeneratorSettings;
public class StoryGenerator
{

    public static void GenerateArc(Power power, float difficultyStart, float difficultyEnd, int puzzleAmount, string name, string guidKey)
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
        for (int i = 0; i < puzzleAmount; i++)
        {
            float difficulty = difficultyStart + (difficultyEnd - difficultyStart)*(i/((float)puzzleAmount-1));
            PuzzleDifficultySetter pds = new PuzzleDifficultySetter(power, difficulty);
            PuzzleGeneratorSettings settings = new PuzzleGeneratorSettings();
            pds.ApplyTo(settings);

            PuzzleGenerator generator = new PuzzleGenerator(settings);
            GeneratorGridData ggd;
            do
            {
                int seed = Random.Range(int.MinValue, int.MaxValue);

                int progress = 0;
                bool stop = false;
                ggd = generator.Generate(ref progress, seed, ref stop);
            } while (ggd == null);
            string gridPrefillText = InputFieldUpdater.GetPrefillGridText(ggd);
            string gridNextText = InputFieldUpdater.GetNextText(ggd);
            string guid = "22f861e77500e3d4b9efaf9bf4f" + guidKey;
            string id = "" + (int)power;
            if (i < 10)
            {
                id += "0" + i;
            }
            else
            {
                id += i;
            }

            guid += id;
            gridPrefillText = gridPrefillText.Replace("\n", "\n\n    ");
            gridNextText = gridNextText.Replace("\n", "\n\n    ");
            arcGuidLines += arcGuidLine.Replace("%guid%", guid);
            string puzzleFile = puzzleFileStructure.Replace("%prefill%", gridPrefillText);
            puzzleFile = puzzleFile.Replace("%next%", gridNextText);
            puzzleFile = puzzleFile.Replace("%powerAtStart%", powerGuids[(int)power]);
            puzzleFile = puzzleFile.Replace("%sizeY%", "10");
            string puzzleMetaFile = puzzleMetaFileStructure.Replace("%guid%", guid);
            File.WriteAllText($"GeneratedPuzzles/puzzle_{name}{id}.asset", puzzleFile);
            File.WriteAllText($"GeneratedPuzzles/puzzle_{name}{id}.asset.meta", puzzleMetaFile);

        }
        string arcFile = arcFileStructure.Replace("%guidLines%", arcGuidLines);
        File.WriteAllText($"GeneratedPuzzles/arc_{name}.asset", arcFile);

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
