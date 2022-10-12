using static PuzzleGeneratorSettings;

public class PuzzleDifficultySetter
{
    private Power power;
    private float difficulty;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="power">The power the puzzle will use</param>
    /// <param name="difficulty">The difficulty as a float (0.0 ~ 7.0).
    /// 0: Even your grandma can do it
    /// 1: Only if you have a higher quality grandma
    /// 2: Grandma may get younger by playing this difficulty and young newcomers should think for a few seconds
    /// 3: Still need a few seconds after playing for an hour
    /// 4: Hardest difficulty puzzle versus should start here. Often has multiple solutions.
    /// 5: Should take us (devs) a few seconds to solve.
    /// 6: Should make us (devs) sweat a bit.
    /// 7: Hardest diffculty puzzle versus should end here. Should be hard for anyone, even after training.
    /// Overall, puzzles with powers should be slightly harder.</param>
    public PuzzleDifficultySetter(Power power, float difficulty)
    {
        this.power = power;
        this.difficulty = difficulty;
    }

    public void ApplyTo(PuzzleGeneratorSettings settings)
    {
        switch (power)
        {
            case Power.NONE:
                ApplyWithPowerNone(settings);
                break;
            case Power.TATANA_CUT:
                ApplyWithPowerTatanaCut(settings);
                break;
            case Power.TATANA_VERTICAL_CUT:
                ApplyWithPowerTatanaVerticalCut(settings);
                break;
            case Power.GRAVITAK_GRAVITY:
                ApplyWithPowerGravitakGravity(settings);
                break;
            case Power.KUKUPIN_WALL:
                ApplyWithPowerKukupinWall(settings);
                break;
            case Power.MOMONI_CLEAN_COLOR:
                ApplyWithPowerMomoniCleanColor(settings);
                break;
            case Power.TUTUT_BICOLOR_PIECE:
                ApplyWithPowerTututBicolorPiece(settings);
                break;
            case Power.BARBAK_GROUP3:
                ApplyWithPowerBarbakGroup3(settings);
                break;
            case Power.YIYIFU_PAINT_BOMB:
                ApplyWithPowerYiyifuPaintBomb(settings);
                break;
            case Power.YIYIFU_OVNI:
                ApplyWithPowerYiyifuOVNI(settings);
                break;

        }
    }

    private void ApplyWithPowerNone(PuzzleGeneratorSettings settings)
    {
        settings.maxHeight = (int)(System.Math.Min(3 + difficulty * 0.9, 10f));
        settings.minHeight = (int)(System.Math.Min(1 + difficulty, 9f));
        settings.colorAmount = (int)(System.Math.Min(3 + difficulty, 6));
        settings.ticksGoal = (int)(3 + difficulty * 0.6f);
        settings.minimumTicks = (int)(2 + difficulty * 0.6f);
        settings.importanceOfTicksGoal = 0.4f;
        settings.importanceOf1BottlePerChain = 0.5f;
        settings.importanceOfHighBottles = 1f;
        settings.importanceOfNotHavingGhosts = System.Math.Max(1 - difficulty / 3f, 0);
        settings.importanceOfClearing = 1f;
        settings.isClearingNecessary = true;
        settings.isNotHavingGhostsNecessary = difficulty < 1f;
        settings.isHavingAllBottlesOnTopNecessary = false;
        settings.isHaving1BottlePerChainNecessary = difficulty < 1.5f;
        settings.isRespectingTicksGoalNecessary = false;
        settings.nextPieceAmount = (int)System.Math.Min(1.5 + difficulty * 0.6f, 4);
        settings.shouldBePlayableWithoutHold = difficulty < 2.5f;
        settings.shouldNextPiecesBeOnlyBottles = false;
        settings.retriesForEachEnhancement = (int)(20 + difficulty * 25);
        settings.enhancementAmount = (int)(20 + difficulty * 25);
        settings.computationTime = 300;
        settings.power = Power.NONE;
    }

    private void ApplyWithPowerTatanaCut(PuzzleGeneratorSettings settings)
    {
        ApplyWithPowerNone(settings);
        settings.isHaving1BottlePerChainNecessary = false;
        settings.power = Power.TATANA_CUT;
    }
    private void ApplyWithPowerTatanaVerticalCut(PuzzleGeneratorSettings settings)
    {
        ApplyWithPowerNone(settings);
        settings.power = Power.TATANA_VERTICAL_CUT;
    }

    private void ApplyWithPowerKukupinWall(PuzzleGeneratorSettings settings)
    {
        ApplyWithPowerNone(settings);
        settings.nextPieceAmount = (int)System.Math.Min(2 + difficulty * 0.6f, 4);
        settings.power = Power.KUKUPIN_WALL;
    }

    private void ApplyWithPowerGravitakGravity(PuzzleGeneratorSettings settings)
    {
        ApplyWithPowerNone(settings);
        settings.power = Power.GRAVITAK_GRAVITY;
    }
    private void ApplyWithPowerMomoniCleanColor(PuzzleGeneratorSettings settings)
    {
        ApplyWithPowerNone(settings);
        settings.minimumTicks--;
        settings.power = Power.MOMONI_CLEAN_COLOR;
    }
    private void ApplyWithPowerTututBicolorPiece(PuzzleGeneratorSettings settings)
    {
        ApplyWithPowerNone(settings);
        settings.nextPieceAmount = (int)System.Math.Min(2 + difficulty * 0.6f, 4);
        settings.minimumTicks -= 2;
        settings.isHaving1BottlePerChainNecessary = false;
        settings.power = Power.TUTUT_BICOLOR_PIECE;
    }
    private void ApplyWithPowerBarbakGroup3(PuzzleGeneratorSettings settings)
    {
        ApplyWithPowerNone(settings);
        settings.power = Power.BARBAK_GROUP3;
    }
    private void ApplyWithPowerYiyifuPaintBomb(PuzzleGeneratorSettings settings)
    {
        ApplyWithPowerNone(settings);
        settings.minimumTicks--;
        settings.power = Power.YIYIFU_PAINT_BOMB;
    }
    private void ApplyWithPowerYiyifuOVNI(PuzzleGeneratorSettings settings)
    {
        ApplyWithPowerNone(settings);
        settings.power = Power.YIYIFU_OVNI;
    }
}
