using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MachineLearning.Autotrainer
{
	internal class Trainer
	{
		readonly string workingDirectory = "";
		readonly string projectDirectory = "";
		readonly string trainingDirectory = "";
		readonly string activationPath = "";

		StringBuilder cmdArgs = new StringBuilder();
		Process cmd = new Process();

		public Trainer()
		{
			workingDirectory = Environment.CurrentDirectory;
			projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;
			trainingDirectory = Path.Combine(projectDirectory, "venv");
			activationPath = Path.Combine("Scripts", "activate");

			cmdArgs.Append("/K cd /d " + trainingDirectory);
			cmdArgs.Append(" && " + activationPath);
		}

		void StartEnvironment()
		{
			cmd.StartInfo.FileName = "cmd.exe";
			cmd.StartInfo.Arguments = cmdArgs.ToString();
			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.RedirectStandardError = true;
			cmd.StartInfo.UseShellExecute = false;
			cmd.StartInfo.CreateNoWindow = false;

			cmd.OutputDataReceived += (sender, args) =>
			{
				if (!string.IsNullOrEmpty(args.Data))
				{
					Console.WriteLine(args.Data); // Print to console for real-time feedback
					LogProgress(args.Data); // Log to file
				}
			};

			cmd.Start();
			cmd.BeginOutputReadLine();
		}

		void StartTraining(ModelParameters parameters)
		{
			using (var writer = cmd.StandardInput)
			{
				if (writer.BaseStream.CanWrite)
				{
					writer.WriteLine($"mlagents-learn config.yaml");
				}
			}

			// Ensure training completes before proceeding
			cmd.WaitForExit();
		}

		void LogProgress(string output)
		{
			string logFilePath = Path.Combine(trainingDirectory, "training_log.txt");
			File.AppendAllText(logFilePath, output);
		}

		void RunTest(ModelParameters parameter)
		{
			StartEnvironment();

			StartTraining(parameter);

			cmd.Close();
			LogParser.Parse(Path.Combine(trainingDirectory, "training_log.txt"), Path.Combine(trainingDirectory, "training_data.csv"));//todo
		}

		static void Main(string[] args)
		{
			var trainer = new Trainer();


			for (int i = 0; i < 4; i++)
			{
				var newModel = new ModelParameters();
				newModel.behaviors["MoveToTarget"].network_settings.num_layers = i;
				newModel.WriteToFile();
				trainer.RunTest(newModel);
			}

		}
	}
}