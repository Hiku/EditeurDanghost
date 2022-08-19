public class PuzzleDifficultySetter
{
    private PuzzleGenerator.Power power;
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
    public PuzzleDifficultySetter(PuzzleGenerator.Power power, float difficulty)
    {
        this.power = power;
        this.difficulty = difficulty;
    }

    public void ApplyTo(PuzzleGenerator generator)
    {
        switch (power)
        {
            case PuzzleGenerator.Power.NONE:
                ApplyWithPowerNone(generator);
                break;
            case PuzzleGenerator.Power.TATANA_CUT:
                ApplyWithPowerTatanaCut(generator);
                break;
            case PuzzleGenerator.Power.GRAVITAK_GRAVITY:
                ApplyWithPowerGravitakGravity(generator);
                break;
            case PuzzleGenerator.Power.KUKUPIN_WALL:
                ApplyWithPowerKukupinWall(generator);
                break;
            case PuzzleGenerator.Power.MOMONI_CLEAN_COLOR:
                ApplyWithPowerMomoniCleanColor(generator);
                break;
            case PuzzleGenerator.Power.TUTUT_BICOLOR_PIECE:
                ApplyWithPowerTututBicolorPiece(generator);
                break;
            case PuzzleGenerator.Power.BARBAK_GROUP3:
                ApplyWithPowerBarbakGroup3(generator);
                break;
            case PuzzleGenerator.Power.YIYIFU_PAINT_BOMB:
                ApplyWithPowerYiyifuPaintBomb(generator);
                break;
            case PuzzleGenerator.Power.YIYIFU_OVNI:
                ApplyWithPowerYiyifuOVNI(generator);
                break;

        }
    }

    private void ApplyWithPowerNone(PuzzleGenerator generator)
    {
        generator.maxHeight = (int)(System.Math.Min(3 + difficulty * 0.9, 10f));
        generator.minHeight = (int)(System.Math.Min(1 + difficulty, 9f));
        generator.colorAmount = (int)(System.Math.Min(3 + difficulty, 6));
        generator.ticksGoal = (int)(3 + difficulty * 0.6f);
        generator.minimumTicks = (int)(2 + difficulty * 0.6f);
        generator.importanceOfTicksGoal = 0.4f;
        generator.importanceOf1BottlePerChain = 0.5f;
        generator.importanceOfHighBottles = 1f;
        generator.importanceOfNotHavingGhosts = System.Math.Max(1 - difficulty / 3f, 0);
        generator.importanceOfClearing = 1f;
        generator.isClearingNecessary = true;
        generator.isNotHavingGhostsNecessary = difficulty < 1f;
        generator.isHavingAllBottlesOnTopNecessary = false;
        generator.isHaving1BottlePerChainNecessary = difficulty < 1.5f;
        generator.isRespectingTicksGoalNecessary = false;
        generator.nextPieceAmount = (int)System.Math.Min(1.5 + difficulty * 0.6f, 4);
        generator.shouldBePlayableWithoutHold = difficulty < 2.5f;
        generator.shouldNextPiecesBeOnlyBottles = false;
        generator.retriesForEachEnhancement = (int)(20 + difficulty * 25);
        generator.enhancementAmount = (int)(20 + difficulty * 25);
        generator.power = PuzzleGenerator.Power.NONE;
    }

    private void ApplyWithPowerTatanaCut(PuzzleGenerator generator)
    {
        ApplyWithPowerNone(generator);
        generator.isHaving1BottlePerChainNecessary = false;
        generator.power = PuzzleGenerator.Power.TATANA_CUT;
    }

    private void ApplyWithPowerKukupinWall(PuzzleGenerator generator)
    {
        ApplyWithPowerNone(generator);
        generator.nextPieceAmount = (int)System.Math.Min(2 + difficulty * 0.6f, 4);
        generator.power = PuzzleGenerator.Power.KUKUPIN_WALL;
    }

    private void ApplyWithPowerGravitakGravity(PuzzleGenerator generator)
    {
        ApplyWithPowerNone(generator);
        generator.power = PuzzleGenerator.Power.GRAVITAK_GRAVITY;
    }
    private void ApplyWithPowerMomoniCleanColor(PuzzleGenerator generator)
    {
        ApplyWithPowerNone(generator);
        generator.minimumTicks--;
        generator.power = PuzzleGenerator.Power.MOMONI_CLEAN_COLOR;
    }
    private void ApplyWithPowerTututBicolorPiece(PuzzleGenerator generator)
    {
        ApplyWithPowerNone(generator);
        generator.nextPieceAmount = (int)System.Math.Min(2 + difficulty * 0.6f, 4);
        generator.minimumTicks-=2;
        generator.isHaving1BottlePerChainNecessary = false;
        generator.power = PuzzleGenerator.Power.TUTUT_BICOLOR_PIECE;
    }
    private void ApplyWithPowerBarbakGroup3(PuzzleGenerator generator)
    {
        ApplyWithPowerNone(generator);
        generator.power = PuzzleGenerator.Power.BARBAK_GROUP3;
    }
    private void ApplyWithPowerYiyifuPaintBomb(PuzzleGenerator generator)
    {
        ApplyWithPowerNone(generator);
        generator.power = PuzzleGenerator.Power.YIYIFU_PAINT_BOMB;
    }
    private void ApplyWithPowerYiyifuOVNI(PuzzleGenerator generator)
    {
        ApplyWithPowerNone(generator);
        generator.power = PuzzleGenerator.Power.YIYIFU_OVNI;
    }
}
