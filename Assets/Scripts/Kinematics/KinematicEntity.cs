using UnityEngine;

namespace Kinematics
{
    public abstract class KinematicEntity : MonoBehaviour
    {
        [SerializeField] 
        protected KinematicCollection kinematicCollection;
        
        public abstract Kinematic CurrentKinematic { get; }

        protected void SubscribeToKinematicCollection()   => kinematicCollection.Add(this);
        protected void UnsubscribeToKinematicCollection() => kinematicCollection.Remove(this);
    }
}