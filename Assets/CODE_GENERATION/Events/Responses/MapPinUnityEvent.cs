using UnityEngine;
using UnityEngine.Events;
using ScriptableObjectArchitecture;

namespace Microsoft.Maps.Unity
{
	[System.Serializable]
	public sealed class MapPinUnityEvent : UnityEvent<MapPin>
	{
	}
}