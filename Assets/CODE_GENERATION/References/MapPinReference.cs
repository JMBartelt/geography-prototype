using UnityEngine;
using ScriptableObjectArchitecture;

namespace Microsoft.Maps.Unity
{
	[System.Serializable]
	public sealed class MapPinReference : BaseReference<MapPin, MapPinVariable>
	{
	    public MapPinReference() : base() { }
	    public MapPinReference(MapPin value) : base(value) { }
	}
}