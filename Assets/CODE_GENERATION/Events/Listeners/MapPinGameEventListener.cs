using UnityEngine;
using ScriptableObjectArchitecture;

namespace Microsoft.Maps.Unity
{
	[AddComponentMenu(SOArchitecture_Utility.EVENT_LISTENER_SUBMENU + "MapPin" + " Event Listener")]
	public sealed class MapPinGameEventListener : BaseGameEventListener<MapPin, MapPinGameEvent, MapPinUnityEvent>
	{
	}
}
