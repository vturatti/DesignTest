using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

namespace Pathfinding {
	/// <summary>
	/// Sets the destination of an AI to the position of a specified object.
	/// This component should be attached to a GameObject together with a movement script such as AIPath, RichAI or AILerp.
	/// This component will then make the AI move towards the <see cref="Target"/> set on this component.
	///
	/// See: <see cref="Pathfinding.IAstarAI.destination"/>
	///
	/// [Open online documentation to see images]
	/// </summary>
	[UniqueComponent(tag = "ai.destination")]
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_destination_setter.php")]
	public class AIDestinationSetter : VersionedMonoBehaviour {
		/// <summary>The object that the AI should move to</summary>
		public Transform Target;
		public IAstarAI AstarAI;

		void OnEnable () {
			AstarAI = GetComponent<IAstarAI>();
			// Update the destination right before searching for a path as well.
			// This is enough in theory, but this script will also update the destination every
			// frame as the destination is used for debugging and may be used for other things by other
			// scripts as well. So it makes sense that it is up to date every frame.
			if (AstarAI != null) AstarAI.onSearchPath += Update;
		}

		void OnDisable () {
			if (AstarAI != null) AstarAI.onSearchPath -= Update;
		}

		/// <summary>Updates the AI's destination every frame</summary>
		void Update () {
            if (Target != null && AstarAI != null)
            {
                AstarAI.destination = Target.position;
                AstarAI.isStopped = false;
            }
            if (Target == null)
            {
                AstarAI.isStopped = true;
            }
		}

        public void SetGameObjectTarget(GameObject gameObject)
        {
            if (gameObject == null)
            {
                Target = null;
                return;
            }
            Target = gameObject.transform;
            AstarAI.isStopped = false;
        } 
	}
}
