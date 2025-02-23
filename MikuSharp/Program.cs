namespace MikuSharp;

/// <summary>
///     The main program class.
/// </summary>
public class Program
{
	/// <summary>
	///     The main entry point of the application.
	/// </summary>
	/// <param name="args">The optional command-line arguments.</param>
	public static void Main(string[]? args = null)
	{
		Log.Logger.Information("Startup!");

		using (var bot = new HatsuneMikuBot())
		{
			HatsuneMikuBot.RegisterEventsAsync().Wait();
			bot.RegisterCommands();
			bot.RunAsync().Wait();
		}

		Log.Logger.Information("Shutdown!");
	}
}
