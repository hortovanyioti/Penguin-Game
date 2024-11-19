using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace MachineLearning.Autotrainer
{
	class LogParser
	{
		public static void Parse(string inputPath, string outputPath)
		{
			char decimalSeparator= Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			// Define a regular expression to match the pattern in each log line
			string pattern = @"Step:\s*(\d+)\.\s*Time\s*Elapsed:\s*([\d.]+)\s*s.\s*Mean\s*Reward:\s*([-+]?\d*\.\d+|\d+).\s*Std\s*of\s*Reward:\s*([-+]?\d*\.\d+|\d+)";
			Regex regex = new Regex(pattern);

			using (StreamWriter outputFile = new StreamWriter(outputPath))
			{
				// Write CSV header
				outputFile.WriteLine("Step;Time(s);MeanReward;StdReward");
				string logContent = File.ReadAllText(inputPath);
				string[] logEntries = logContent.Split(new string[] { "[INFO]" }, StringSplitOptions.RemoveEmptyEntries);

				foreach (var entry in logEntries)
				{
					Match match = regex.Match(entry);
					if (match.Success)
					{
						// Extract values using regex groups
						string step = match.Groups[1].Value;
						string time = match.Groups[2].Value;
						string meanReward = match.Groups[3].Value;
						string stdReward = match.Groups[4].Value;

						if(decimalSeparator != '.')
						{
							// Replace the decimal separator if necessary
							time = time.Replace('.',decimalSeparator);
							meanReward = meanReward.Replace('.',decimalSeparator);
							stdReward = stdReward.Replace('.',decimalSeparator);
						}

						// Write the parsed data to the CSV
						outputFile.WriteLine($"{step};{time};{meanReward};{stdReward}");
					}
				}
			}
		}
	}
}
