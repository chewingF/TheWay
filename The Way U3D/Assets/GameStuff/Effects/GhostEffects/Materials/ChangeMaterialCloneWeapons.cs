using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cakeslice
{
    public class ChangeMaterialCloneWeapons : MonoBehaviour
    {
        public Material ghostMaterial;

        void Start()
        {
            if (this.gameObject.transform.root.tag == "RecordedPlayer")
            {
				if (ghostMaterial != null)
                	this.gameObject.GetComponent<Renderer>().material = ghostMaterial;
				else
					Debug.Log("No Ghost Material was found for " + this.gameObject.name);
				
				this.gameObject.AddComponent<Outline>();
            }
        }
    }
}
