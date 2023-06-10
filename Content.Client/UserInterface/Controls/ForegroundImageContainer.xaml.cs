using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.UserInterface.Controls
{
    [GenerateTypedNameReferences]
    public partial class ForegroundImageContainer : Container //<todo.eoin Rename this
    {
        // The least amount of margin that a child needs to have to avoid drawing under
        // undesirable parts of the images. Children can add larger margins if desired
        public const string StylePropertyContentMargin = "MinimumContentMargin"; //<todo.eoin Rename

        // The stylebox used to draw the foreground
        public const string StylePropertyForegroundStyleBox = "ForegroundStyleBox";
        // An extra modulation color for the foreground style box
        public const string StylePropertyForegroundModulate = "ForegroundModulate";

        // The stylebox used to draw the background
        public const string StylePropertyBackgroundStyleBox = "BackgroundStyleBox";
        // An extra modulation color for the background style box
        public const string StylePropertyBackgroundModulate = "BackgroundModulate";

        public Color? ForegroundModulate
        {
            get => ForegroundContainer.ModulateSelfOverride;
            set => ForegroundContainer.ModulateSelfOverride = value;
        }

        public Color? BackgroundModulate
        {
            get => ForegroundContainer.ModulateSelfOverride;
            set => ForegroundContainer.ModulateSelfOverride = value;
        }

        public ForegroundImageContainer()
        {
            RobustXamlLoader.Load(this);
            XamlChildren = ContentsContainer.Children;
        }

        protected override void StylePropertiesChanged()
        {
            if (TryGetStyleProperty<StyleBox>(StylePropertyForegroundStyleBox, out var foregroundStyleBox))
            {
                ForegroundContainer.PanelOverride = foregroundStyleBox;
            }

            if (TryGetStyleProperty<Color>(StylePropertyForegroundModulate, out var foregroundModulate))
            {
                ForegroundContainer.ModulateSelfOverride = foregroundModulate;
            }

            if (TryGetStyleProperty<Thickness>(StylePropertyContentMargin, out var contentMargin))
            {
                ContentsContainer.Margin = contentMargin;
            }

            if (TryGetStyleProperty<StyleBox>(StylePropertyBackgroundStyleBox, out var backgroundStyleBox))
            {
                BackgroundContainer.PanelOverride = backgroundStyleBox;
            }

            if (TryGetStyleProperty<Color>(StylePropertyBackgroundModulate, out var backgroundModulate))
            {
                BackgroundContainer.ModulateSelfOverride = backgroundModulate;
            }
         }

    }
}
