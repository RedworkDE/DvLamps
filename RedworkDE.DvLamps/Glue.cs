using System.Reflection;

namespace RedworkDE.DvLamps
{
	public class Glue
	{
		[AutoLoad]
		static void Init()
		{
			// register key binding
			var kb = typeof(FlashlightNonVR).GetField(nameof(FlashlightNonVR.toggleFlashLightKeys));
			KeyBindings.AllKeyBindFields.Add(kb);
			KeyBindings.AllChangableKeyBindFields.Add(kb);

			FlashlightNonVR.ThrowGrabber = grabber =>
#if PUBLICIZER_DV_INTERACTION
				grabber.fsm.Fire(Grabber.Trigger.Throw);
#else
			{
				var fsmField = typeof(Grabber).GetField("fsm", BindingFlags.Instance|BindingFlags.NonPublic);
				var fsm = fsmField.GetValue(grabber);
				fsmField.FieldType.GetMethod("Fire", new [] {fsmField.FieldType.GenericTypeArguments[1]}).Invoke(fsm, new object[] {5});
			};
#endif
		}
	}
}