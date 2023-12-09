using UnityEngine;
using UnityEngine.Events;
using ScriptableObjectArchitecture;

namespace Microsoft.Maps.Unity
{
	[System.Serializable]
	public class MapPinEvent : UnityEvent<MapPin> { }

	[CreateAssetMenu(
	    fileName = "MapPinVariable.asset",
	    menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "MapPin",
	    order = 120)]
	public class MapPinVariable : BaseVariable<MapPin, MapPinEvent>
	{
	}
}