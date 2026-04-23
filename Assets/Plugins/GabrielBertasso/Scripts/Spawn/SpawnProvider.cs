namespace GabrielBertasso.Spawn
{
    public static class SpawnProvider
    {
        public static ISpawner Instance { get; set; } = new DefaultSpawner();
    }
}