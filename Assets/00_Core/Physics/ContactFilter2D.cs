using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
	public struct ContactFilter2D
	{
		//UnityEngine.ContactFilter2D
		public bool useLayerMask;
		public LayerMask layerMask;

		public void ClearLayerMask()
		{
			layerMask = 0;
		}
		public bool IsFilteringLayerMask(GameObject obj)
		{
			return (layerMask & (1 << obj.layer)) == 0;
		}
		public ContactFilter2D NoFilter()
		{
			return new ContactFilter2D();
		}
		public void SetLayerMask(LayerMask layerMask)
		{
			this.layerMask = layerMask;
		}
	}
}