using static WolvenKit.RED4.Types.Enums;

namespace WolvenKit.RED4.Types
{
	public partial class ItemTooltipWeaponInfoModule : ItemTooltipModuleController
	{
		[Ordinal(2)] 
		[RED("wrapper")] 
		public inkWidgetReference Wrapper
		{
			get => GetPropertyValue<inkWidgetReference>();
			set => SetPropertyValue<inkWidgetReference>(value);
		}

		[Ordinal(3)] 
		[RED("arrow")] 
		public inkImageWidgetReference Arrow
		{
			get => GetPropertyValue<inkImageWidgetReference>();
			set => SetPropertyValue<inkImageWidgetReference>(value);
		}

		[Ordinal(4)] 
		[RED("dpsText")] 
		public inkTextWidgetReference DpsText
		{
			get => GetPropertyValue<inkTextWidgetReference>();
			set => SetPropertyValue<inkTextWidgetReference>(value);
		}

		[Ordinal(5)] 
		[RED("perHitText")] 
		public inkTextWidgetReference PerHitText
		{
			get => GetPropertyValue<inkTextWidgetReference>();
			set => SetPropertyValue<inkTextWidgetReference>(value);
		}

		[Ordinal(6)] 
		[RED("attacksPerSecondText")] 
		public inkTextWidgetReference AttacksPerSecondText
		{
			get => GetPropertyValue<inkTextWidgetReference>();
			set => SetPropertyValue<inkTextWidgetReference>(value);
		}

		[Ordinal(7)] 
		[RED("nonLethal")] 
		public inkTextWidgetReference NonLethal
		{
			get => GetPropertyValue<inkTextWidgetReference>();
			set => SetPropertyValue<inkTextWidgetReference>(value);
		}

		[Ordinal(8)] 
		[RED("scopeIndicator")] 
		public inkWidgetReference ScopeIndicator
		{
			get => GetPropertyValue<inkWidgetReference>();
			set => SetPropertyValue<inkWidgetReference>(value);
		}

		[Ordinal(9)] 
		[RED("silencerIndicator")] 
		public inkWidgetReference SilencerIndicator
		{
			get => GetPropertyValue<inkWidgetReference>();
			set => SetPropertyValue<inkWidgetReference>(value);
		}

		[Ordinal(10)] 
		[RED("ammoText")] 
		public inkTextWidgetReference AmmoText
		{
			get => GetPropertyValue<inkTextWidgetReference>();
			set => SetPropertyValue<inkTextWidgetReference>(value);
		}

		[Ordinal(11)] 
		[RED("ammoWrapper")] 
		public inkWidgetReference AmmoWrapper
		{
			get => GetPropertyValue<inkWidgetReference>();
			set => SetPropertyValue<inkWidgetReference>(value);
		}

		public ItemTooltipWeaponInfoModule()
		{
			Wrapper = new();
			Arrow = new();
			DpsText = new();
			PerHitText = new();
			AttacksPerSecondText = new();
			NonLethal = new();
			ScopeIndicator = new();
			SilencerIndicator = new();
			AmmoText = new();
			AmmoWrapper = new();

			PostConstruct();
		}

		partial void PostConstruct();
	}
}
