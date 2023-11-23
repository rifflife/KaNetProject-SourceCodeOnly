using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PluggableAI
{
    public interface IPatrolable
    {
        public Vector3 StartPosition { get; set; }  
        public bool IsReturn { get; set; }
        public Vector3 PatrolDistance { get; set; }   
        
    }

    public interface IChaseTargetable
    {
        public float ChaseTargetRadius { get; }
        public Transform Target { get; set; }
    }

    public interface IAttackTargetable
    {
        public float AttackTargetRadius { get; set; }
        public Transform AttackTarget { get; set; }
    }


}