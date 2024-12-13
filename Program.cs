using System.Text.RegularExpressions;

namespace AIBot1;

internal abstract class Program
{
	private const string Version = "1.0.0";

	private class KeywordEntry(string[] keywords, string[] answers, int? priority = null)
	{
		public string[] Keywords { get; } = keywords;
		public string[] Answers { get; } = answers;
		public int? Priority { get; } = priority;
	}

	private static readonly Random Rnd = new();

	private static string? PickRandomAnswer(string[] answers)
	{
		switch (answers.Length)
		{
			case 0:
				return null;
			case 1:
				return answers[0];
			default:
				var idx = Rnd.NextInt64(0, answers.Length);
				var answer = answers[idx];
				return string.IsNullOrWhiteSpace(answer) ? null : answer;
		}
	}

	private static readonly List<KeywordEntry> KeywordResponses = [];
	private static readonly List<string> QuitWords = ["goodbye", "bye", "quit", "exit"];

	private static bool _botInitialized;

	private static void InitBot()
	{
		KeywordResponses.Add(new KeywordEntry(["hello"],
			["Hello! How can I help you?", "Hi there, what's on your mind?"]));
		KeywordResponses.Add(new KeywordEntry(["you"], ["Let's talk about you", "I am just a bot"]));
		KeywordResponses.Add(new KeywordEntry(["what"], ["What do you think?"]));
		KeywordResponses.Add(new KeywordEntry(["what", "name"], ["I'm Bot.", "Hi! My name is Bot."]));
		KeywordResponses.Add(new KeywordEntry(["i", "am"],
			["Are you sure you are?", "Has it always been this way?", "You are?", "How does that make you feel?"]));
		KeywordResponses.Add(new KeywordEntry(["i", "did"], ["How did that make you feel?"]));
		KeywordResponses.Add(new KeywordEntry(["i", "did", "not"], ["Do you think it is important for you to?"]));
		KeywordResponses.Add(new KeywordEntry(["i", "am", "not"],
			["Why not?", "You are being negative.", "You are not?"]));
		KeywordResponses.Add(new KeywordEntry(["i", "was"], ["You were?", "How did that make you feel?"]));
		KeywordResponses.Add(new KeywordEntry(["i", "was", "not"], ["How did that make you feel?", "Why not?"]));
		KeywordResponses.Add(new KeywordEntry(["not"], ["Why not?", "You are being negative."]));
		KeywordResponses.Add(new KeywordEntry(["no"], ["You are being negative."]));
		KeywordResponses.Add(new KeywordEntry(["who"], ["I am not really sure."]));
		KeywordResponses.Add(new KeywordEntry(["created"], []));
		KeywordResponses.Add(new KeywordEntry(["you", "created", "who"], ["I was created by ciao1092 on Dec 13 2024."]));

		_botInitialized = true;
		Console.WriteLine($"Hello, I am Bot version \"{Version}\".");
	}

	private static void RunBot()
	{
		if (!_botInitialized) throw new InvalidOperationException($"Not initialized. Use InitBot() first");
		while (Console.ReadLine() is { } userInput)
		{
			// Convert to lowercase and remove unwanted characters (simplifying)
			var processedInput = Regex.Replace(userInput.ToLower(), @"[^a-zA-Z\s]", string.Empty);
			var inputWords = processedInput.Split(" ");

			if (QuitWords.Contains(processedInput))
			{
				Console.WriteLine(PickRandomAnswer([
					"Bye", "Bye.", "See you soon!"
				]));
				break;
			}

			var bestMatchIdx = -1;
			float bestMatchQuality = 0;
			for (var i = 0; i < KeywordResponses.Count; i++)
			{
				var k = KeywordResponses.ElementAt(i);
				float currentMatchQuality = inputWords.Count(word => k.Keywords.Contains(word));
				currentMatchQuality /= k.Keywords.Length;

				if (currentMatchQuality > bestMatchQuality)
				{
					ReplaceCurrentBest();
				}
				else if (Math.Abs(currentMatchQuality - bestMatchQuality) == 0 && bestMatchQuality != 0)
				{
					var bestMatch = KeywordResponses.ElementAt(bestMatchIdx);
					if (k.Priority > bestMatch.Priority) ReplaceCurrentBest();
					else if (k.Priority == bestMatch.Priority)
					{
						if (k.Keywords.Length > bestMatch.Keywords.Length) ReplaceCurrentBest();
					}
				}

				continue;

				void ReplaceCurrentBest()
				{
					bestMatchIdx = i;
					bestMatchQuality = currentMatchQuality;
				}
			}

			string? answer = null;
			if (bestMatchIdx >= 0 && bestMatchQuality > 0)
				answer = PickRandomAnswer(KeywordResponses.ElementAt(bestMatchIdx).Answers);

			Console.WriteLine(answer ?? PickRandomAnswer(["Please continue.", "I see."]));
		}
	}

	public static void Main(string[] args)
	{
		InitBot();
		RunBot();
	}
}