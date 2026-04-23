using UnityEngine.AI;

namespace GabrielBertasso.Extensions
{
    public static class NavMeshExtensions
    {
        public static bool IsAtDestination(this NavMeshAgent agent)
        {
            return agent.hasPath && agent.remainingDistance <= agent.stoppingDistance + 0.05f;
        }
    }
}