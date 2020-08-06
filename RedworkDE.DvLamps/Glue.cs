using System.Linq;

namespace RedworkDE.DvLamps
{
	public class Glue:AutoLoad<Glue>
	{
		static Glue()
		{

			// register key binding
			var kb = typeof(FlashlightNonVR).GetField(nameof(FlashlightNonVR.toggleFlashLightKeys));
			KeyBindings.AllKeyBindFields.Add(kb);
			KeyBindings.AllChangableKeyBindFields.Add(kb);

			FlashlightNonVR.ThrowGrabber = grabber => grabber.fsm.Fire(Grabber.Trigger.Throw);
		}
	}
}