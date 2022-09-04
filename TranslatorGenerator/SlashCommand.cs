using System.Windows;
using System.Windows.Media;

namespace DisCatSharp.TranslationGenerator;

public class SlashCommand : ApplicationCommand
{
	static SlashCommand()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(SlashCommand), new FrameworkPropertyMetadata(typeof(SlashCommand)));
	}

	public static readonly DependencyProperty ImagePropery =
			DependencyProperty.Register(
				 "ImageSource", typeof(ImageSource), typeof(SlashCommand),
				 new PropertyMetadata(new PropertyChangedCallback(ImageSourceChangedCallback)));

	public ImageSource ImageSource
	{
		get
		{
			return (ImageSource)GetValue(ImagePropery);
		}

		set
		{
			SetValue(ImagePropery, value);
		}
	}

	private static void ImageSourceChangedCallback(DependencyObject obj,
		DependencyPropertyChangedEventArgs args)
	{
		SlashCommand ctl = (SlashCommand)obj;
		ImageSource newValue = (ImageSource)args.NewValue;

		// Call UpdateStates because the Value might have caused the
		// control to change ValueStates.
		ctl.UpdateStates(true);

		// Call OnValueChanged to raise the ValueChanged event.
		ctl.OnImageSourceChanged(
			new ImageSourceChangedEventArgs(ImageSourceChangedEvent,
				newValue));
	}

	public static readonly RoutedEvent ImageSourceChangedEvent =
		EventManager.RegisterRoutedEvent("ImageSourceChanged", RoutingStrategy.Direct,
					  typeof(ImageSourceChangedEventHandler), typeof(ApplicationCommand));

	public event ImageSourceChangedEventHandler ImageSourceChanged
	{
		add { AddHandler(ImageSourceChangedEvent, value); }
		remove { RemoveHandler(ImageSourceChangedEvent, value); }
	}

	protected virtual void OnImageSourceChanged(ImageSourceChangedEventArgs e)
	{
		RaiseEvent(e);
	}

	public delegate void ImageSourceChangedEventHandler(object sender, ImageSourceChangedEventArgs e);
	public class ImageSourceChangedEventArgs : RoutedEventArgs
	{
		private ImageSource _value;

		public ImageSourceChangedEventArgs(RoutedEvent id, ImageSource val)
		{
			_value = val;
			RoutedEvent = id;
		}

		public ImageSource Value
		{
			get { return _value; }
		}
	}
}
