using UnityEngine;

namespace Kinematics
{
    public abstract class KinematicEntity : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected KinematicCollection kinematicCollection;

        protected Kinematic currentKinematic;
        public abstract ref readonly Kinematic CurrentKinematic { get; }
        
        
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