using System;
using System.Collections.Generic;
using UnityEngine;

namespace GabrielBertasso.Physics
{
    [Serializable]
    public struct DetectionMask
    {
        public LayerMask LayerMask;
        public List<string> Tags;
        public QueryTriggerInteraction TriggerInteraction;

        public DetectionMask(LayerMask layerMask, List<string> tags, QueryTriggerInteraction triggerInteraction)
        {
            LayerMask = layerMask;
            Tags = tags != null ? tags : new List<string>();
            TriggerInteraction = triggerInteraction;
        }
    }
}