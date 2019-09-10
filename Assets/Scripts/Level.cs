public class Level
{
    public int LevelNumber { get; set; }

    public Level()
    {
    }

    public Level(int levelNumber, bool unlocked, bool completed)
    {
        LevelNumber = levelNumber;
        Unlocked = unlocked;
        Completed = completed;
    }

    public bool Unlocked { get; set; }

    public bool Completed { get; set; }
}