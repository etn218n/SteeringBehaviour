using UnityEngine;
using System.Collections.Generic;

namespace Kinematics
{
    [CreateAssetMenu(fileName = "Kinematic Collection.asset")]
    public class KinematicCollection : ScriptableObject
    {
        [SerializeField] 
        private List<KinematicEntity> kinematicEntities = new List<KinematicEntity>();
        public  List<KinematicEntity> KinematicEntities => kinematicEntities;

        
        public void Add(KinematicEntity newEntity)
        {
            if (kinematicEntities.Contains(newEntity))
                return;
            
            kinematicEntities.Add(newEntity);
        }
        
        
        public void Remove(KinematicEntity newEntity)
        {
            kinematicEntities.Remove(newEntity);
        }
    }
}