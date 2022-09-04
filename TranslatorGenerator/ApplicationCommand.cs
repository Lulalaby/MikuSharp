using System.Windows;
using System.Windows.Controls;

namespace DisCatSharp.TranslationGenerator;

public class ApplicationCommand : UserControl
{
	static ApplicationCommand()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(ApplicationCommand), new FrameworkPropertyMetadata(typeof(ApplicationCommand)));
	}

	public static readonly DependencyProperty ACNameProperty =
			DependencyProperty.Register(
				"ACName", typeof(string), typeof(ApplicationCommand),
				new PropertyMetadata(
					new PropertyChangedCallback(ACNameChangedCallback)), new ValidateValueCallback(ValidContent));

	public static readonly DependencyProperty ACDescProperty =
			DependencyProperty.Register(
				"ACDesc", typeof(string), typeof(ApplicationCommand),
				new PropertyMetadata(
					new PropertyChangedCallback(ACDescChangedCallback)), new ValidateValueCallback(ValidContent));

	public string ACName
	{
		get
		{
			return (string)GetValue(ACNameProperty);
		}

		set
		{
			SetValue(ACNameProperty, value);
		}
	}

	public string ACDesc
	{
		get
		{
			return (string)GetValue(ACDescProperty);
		}

		set
		{
			SetValue(ACDescProperty, value);
		}
	}

	private static void ACNameChangedCallback(DependencyObject obj,
			DependencyPropertyChangedEventArgs args)
	{
		ApplicationCommand ctl = (ApplicationCommand)obj;
		string newValue = (string)args.NewValue;

		// Call UpdateStates because the Value might have caused the
		// control to change ValueStates.
		ctl.UpdateStates(true);

		// Call OnValueChanged to raise the ValueChanged event.
		ctl.OnACNameChanged(
			new ACNameChangedEventArgs(ACNameChangedEvent,
				newValue));
	}

	private static void ACDescChangedCallback(DependencyObject obj,
			DependencyPropertyChangedEventArgs args)
	{
		ApplicationCommand ctl = (ApplicationCommand)obj;
		string newValue = (string)args.NewValue;

		// Call UpdateStates because the Value might have caused the
		// control to change ValueStates.
		ctl.UpdateStates(true);

		// Call OnValueChanged to raise the ValueChanged event.
		ctl.OnACDescChanged(
			new ACDescChangedEventArgs(ACDescChangedEvent,
				newValue));
	}

	private static bool ValidContent(object value)
	{
		return true;
	}

	protected override void OnGotFocus(RoutedEventArgs e)
	{
		base.OnGotFocus(e);
		UpdateStates(true);
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		base.OnLostFocus(e);
		UpdateStates(true);
	}

	internal void UpdateStates(bool useTransitions)
	{
		if (IsFocused)
		{
			VisualStateManager.GoToState(this, "Focused", useTransitions);
		}
		else
		{
			VisualStateManager.GoToState(this, "Unfocused", useTransitions);
		}
	}

	public static readonly RoutedEvent ACNameChangedEvent =
		EventManager.RegisterRoutedEvent("ACNameChanged", RoutingStrategy.Direct,
					  typeof(ACNameChangedEventHandler), typeof(ApplicationCommand));

	public static readonly RoutedEvent ACDescChangedEvent =
		EventManager.RegisterRoutedEvent("ACDescChanged", RoutingStrategy.Direct,
					  typeof(ACDescChangedEventHandler), typeof(ApplicationCommand));

	public event ACNameChangedEventHandler ACNameChanged
	{
		add { AddHandler(ACNameChangedEvent, value); }
		remove { RemoveHandler(ACNameChangedEvent, value); }
	}

	public event ACDescChangedEventHandler ACDescChanged
	{
		add { AddHandler(ACDescChangedEvent, value); }
		remove { RemoveHandler(ACDescChangedEvent, value); }
	}

	protected virtual void OnACNameChanged(ACNameChangedEventArgs e)
	{
		// Raise the ValueChanged event so applications can be alerted
		// when Value changes.
		RaiseEvent(e);
	}

	protected virtual void OnACDescChanged(ACDescChangedEventArgs e)
	{
		// Raise the ValueChanged event so applications can be alerted
		// when Value changes.
		RaiseEvent(e);
	}

	public delegate void ACNameChangedEventHandler(object sender, ACNameChangedEventArgs e);
	public delegate void ACDescChangedEventHandler(object sender, ACDescChangedEventArgs e);

	public class ACNameChangedEventArgs : RoutedEventArgs
	{
		private string _value;

		public ACNameChangedEventArgs(RoutedEvent id, string val)
		{
			_value = val;
			RoutedEvent = id;
		}

		public string Value
		{
			get { return _value; }
		}
	}

	public class ACDescChangedEventArgs : RoutedEventArgs
	{
		private string _value;

		public ACDescChangedEventArgs(RoutedEvent id, string val)
		{
			_value = val;
			RoutedEvent = id;
		}

		public string Value
		{
			get { return _value; }
		}
	}

}
