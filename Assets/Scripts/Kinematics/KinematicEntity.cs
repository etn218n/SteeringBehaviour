using UnityEngine;

namespace Kinematics
{
    public abstract class KinematicEntity : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected KinematicCollection kinematicCollection;
        
        public abstract Kinematic CurrentKinematic { get; }
        
        
        protected virtual void OnEnable()
        {
            if (kinematicCollection != null)
                kinematicCollection.Add(this);
        }
        

        protected virtual void OnDisable()
        {
            if (kinematicCollection != null)
                kinematicCollection.Remove(this);
        }
    }
}