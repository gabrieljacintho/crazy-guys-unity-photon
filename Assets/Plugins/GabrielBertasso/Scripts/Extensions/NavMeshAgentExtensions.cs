using UnityEngine;
using UnityEngine.AI;

namespace GabrielBertasso.Extensions
{
    public static class NavMeshAgentExtensions
    {
        public static bool IsAtDestination(this NavMeshAgent agent)
        {
            if (agent.pathPending)
            {
                return false;
            }

            float stoppingDistance = agent.stoppingDistance + 0.05f;
            if (agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                return agent.remainingDistance <= stoppingDistance;
            }

            return Vector3.Distance(agent.transform.position, agent.destination) <= stoppingDistance;
        }
    }
}