using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Kinematics
{
    [CreateAssetMenu(fileName = "Kinematic Collection.asset")]
    public class KinematicCollection : ScriptableObject, IEnumerable<KinematicEntity>
    {
        private List<KinematicEntity> kinematicEntities = new List<KinematicEntity>();

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

        public IEnumerator<KinematicEntity> GetEnumerator()
        {
            return kinematicEntities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}