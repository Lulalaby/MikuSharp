using System.Windows;

namespace DisCatSharp.TranslationGenerator;

public class ContextMenuCommand : ApplicationCommand
{
	static ContextMenuCommand()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenuCommand), new FrameworkPropertyMetadata(typeof(ContextMenuCommand)));
	}
}
