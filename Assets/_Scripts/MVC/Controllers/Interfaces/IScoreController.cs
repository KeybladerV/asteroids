namespace MVC.Controllers.Interfaces
{
    public interface IScoreController
    {
        int Score { get; }
        
        void AddScore(int score);
        void ResetScore();
    }
}