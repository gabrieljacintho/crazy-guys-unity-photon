namespace Quantum
{
    public partial struct Coin
    {
        public bool IsActive(Frame frame)
        {
            return !RefreshTimer.IsRunning(frame);
        }
    }
}