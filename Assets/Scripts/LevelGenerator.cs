using UnityEngine;

public class LevelGenerator
{
    Brick.RollDirection[] allDirections = new Brick.RollDirection[]{ Brick.RollDirection.LEFT,
                                                                     Brick.RollDirection.TOP,
                                                                     Brick.RollDirection.RIGHT,
                                                                     Brick.RollDirection.BOTTOM }; 

    /**
    * Generate a level with an approximate amount of actions
    **/
    public void GenerateLevel(int actionCount)
    {
        Brick.RollDirection[] sequence = GenerateSequenceOfActions(actionCount);
        for (int i = 0; i != sequence.Length; i++)
        {
            Debug.Log(sequence[i]);
        }
    }

    private Brick.RollDirection[] GenerateSequenceOfActions(int actionCount)
    {
        Brick.RollDirection[] sequence = new Brick.RollDirection[actionCount];
        int i = 0;
        while (i < actionCount)
        {
            sequence[i] = GetRandomDirection(FindAvailableDirections(i > 0 ? sequence[i-1] : Brick.RollDirection.NONE));
            i++;
        }

        return sequence;
    }

    /**
    * Pick up a random direction among directions that are available at this state of the game
    **/
    private Brick.RollDirection GetRandomDirection(Brick.RollDirection[] availableDirections)
    {
        int rand = Random.Range(0, availableDirections.Length);

        return availableDirections[rand];
    }

    /**
    * 
    **/
    private Brick.RollDirection[] FindAvailableDirections(Brick.RollDirection prevAction)
    {
        Brick.RollDirection[] availableDirections = new Brick.RollDirection[3];

        Brick.RollDirection oppositeAction = GetOppositeDirection(prevAction);
        int index = 0;
        for (int i = 0; i != allDirections.Length; i++)
        {
            if (allDirections[i] != oppositeAction)
            {
                availableDirections[index] = allDirections[i];
                index++;
            }
        }

        return availableDirections;
    }

    private Brick.RollDirection GetOppositeDirection(Brick.RollDirection direction)
    {
        if (direction == Brick.RollDirection.LEFT)
            return Brick.RollDirection.RIGHT;
        else if (direction == Brick.RollDirection.RIGHT)
            return Brick.RollDirection.LEFT;
        else if (direction == Brick.RollDirection.TOP)
            return Brick.RollDirection.BOTTOM;
        else
            return Brick.RollDirection.TOP;
    }
}
