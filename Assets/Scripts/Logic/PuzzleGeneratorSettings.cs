using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGeneratorSettings
{
    public enum Power
    {
        NONE,
        TATANA_CUT,
        TATANA_VERTICAL_CUT,
        TUTUT_BICOLOR_PIECE,
        KUKUPIN_WALL,
        YIYIFU_PAINT_BOMB,
        YIYIFU_OVNI,
        MOMONI_CLEAN_COLOR,
        BARBAK_GROUP3,
        GRAVITAK_GRAVITY,
        GU_ARMOR,
        KUSHINAK_STICKY,
    }

    // The maximum height (inclusive) of the puzzle grid columns
    public float maxHeight;
    // The minimum height (inclusive) of the puzzle grid columns
    public float minHeight;
    // The amount of colors in the puzzle
    public int colorAmount;
    // The tick amount that the generator tries to satisfy
    public int ticksGoal;

    // Importances are 0: don't try, 1: default importance, and they can be anything above 0
    public float importanceOfTicksGoal;
    // 1 bottle per chain is about not having too many bottles popping at the same time, which might be confusing and look inefficient
    public float importanceOf1BottlePerChain;
    // Having high bottles makes it feel kind of as if you were playing the real game; you mostly put bottles on top of generated danghosts
    public float importanceOfHighBottles;
    // Ghosts can be confusing for beginers, so it might be better to only have only few of them in easy puzzles
    public float importanceOfNotHavingGhosts;
    // Puzzles should be clearable for now
    public float importanceOfClearing;

    // If the conditions set to true are not satisfied, the puzzle will be generated again
    public bool isClearingNecessary;
    public bool isNotHavingGhostsNecessary;
    public bool isHavingAllBottlesOnTopNecessary;
    public bool isHaving1BottlePerChainNecessary;
    public bool isRespectingTicksGoalNecessary;
    // If the tick amount is lower than the minimum, the puzzle will be generated again
    public int minimumTicks;
    // The amount of pieces the player should use for the puzzle
    public int nextPieceAmount;
    // If the puzzle should be playable without hold, the generator will place pieces in next straightforwardly, which is easier for the player
    public bool shouldBePlayableWithoutHold;
    // It might be easier for the player to only have to place pieces as bottles, for the first puzzles
    public bool shouldNextPiecesBeOnlyBottles;
    // How many ways of enhancing the grid will be tried 
    public int retriesForEachEnhancement;
    // How many times the grid will be enhanced
    public int enhancementAmount;
    // After how much time should the computation stop
    public float computationTime;

    // The power the player has to deal with in the puzzle
    public Power power;
}
