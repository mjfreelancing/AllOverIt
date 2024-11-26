namespace PropertyNavigationDemo.Models
{
    public class Level1
    {
        public Level2 Level2 { get; } = new();
    }
}