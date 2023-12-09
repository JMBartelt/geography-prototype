using UnityEngine;
using ScriptableObjectArchitecture;

namespace Microsoft.Maps.Unity
{
	[System.Serializable]
	[CreateAssetMenu(
	    fileName = "MapPinGameEvent.asset",
	    menuName = SOArchitecture_Utility.GAME_EVENT + "MapPin",
	    order = 120)]
	public sealed class MapPinGameEvent : GameEventBase<MapPin>
	{
	}
}