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
		readonly string trainingResultsDirectory = "";
		readonly string activationPath = "";
		string modelDirectory = "";

		Model model = new Model();
		StringBuilder cmdArgs = new StringBuilder();
		Process cmd = new Process();
		ManualResetEvent processExitEvent = new ManualResetEvent(false);

		public Trainer()
		{
			workingDirectory = Environment.CurrentDirectory;
			projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;
			trainingDirectory = Path.Combine(projectDirectory, "venv");
			trainingResultsDirectory = Path.Combine(trainingDirectory, "results");
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
			cmd.EnableRaisingEvents = true;
			cmd.Exited += (sender, args) => processExitEvent.Set();

			cmd.OutputDataReceived += (sender, args) =>
			{
				if (!string.IsNullOrEmpty(args.Data))
				{
					Console.WriteLine(args.Data); // Print to console for real-time feedback
					LogProgress(model.checkpoint_settings.run_id, args.Data); // Log to file
				}
			};

			cmd.ErrorDataReceived += (sender, args) =>
			{
				if (!string.IsNullOrEmpty(args.Data))
				{
					Console.WriteLine("ERROR: " + args.Data); // Print errors to console
				}
			};

			cmd.Start();
			cmd.BeginOutputReadLine();
			cmd.BeginErrorReadLine();
		}

		void StartTraining()
		{
			var writer = cmd.StandardInput;
			if (writer.BaseStream.CanWrite)
			{
				writer.WriteLine($"mlagents-learn config.yaml");
			}

			//cmd.WaitForExit();

			var finished = false;
			while (!finished)
			{
				if (File.Exists(Path.Combine(modelDirectory, "configuration.yaml")))
				{
					finished = true;
				}
				else
				{
					Thread.Sleep(100);
				}
			}
		}

		void LogProgress(string fileNameWithoutExtension, string data)
		{
			string logFilePath = Path.Combine(modelDirectory, fileNameWithoutExtension + ".txt");

			var success = false;
			while (!success)  //Need to wait for the directory to be created by the training process (DO NOT CREATE MANUALLY)
			{
				try
				{
					File.AppendAllText(logFilePath, data + Environment.NewLine);

					success = true; // If no exception, mark as success
				}
				catch (Exception ex)
				{
					Thread.Sleep(100);
				}
			}
		}

		void RunTest()
		{
			StartEnvironment();
			StartTraining();

			cmd.Close();
			LogParser.Parse(Path.Combine(modelDirectory, model.checkpoint_settings.run_id + ".txt"), Path.Combine(modelDirectory, model.checkpoint_settings.run_id + ".csv"));
		}

		static void Main(string[] args)
		{
			var behaviorName = "MoveToTarget";

			for (int i = 0; i < 4; i++)
			{
				var trainer = new Trainer();
				trainer.model = new Model();
				trainer.model.engine_settings.time_scale = 50;
				trainer.model.env_settings.num_envs= 10;
				trainer.model.behaviors[behaviorName].checkpoint_interval = 1000000;
				trainer.model.behaviors[behaviorName].max_steps = 6000000;
				trainer.model.behaviors[behaviorName].summary_freq = 60000;
				trainer.model.behaviors[behaviorName].network_settings.num_layers = 3;
				trainer.model.torch_settings.device = "cpu";

				trainer.model.checkpoint_settings.run_id = "env_overload_10_env_4thread";

				trainer.model.WriteToFile(Path.Combine(trainer.trainingDirectory, "config.yaml"));
				trainer.modelDirectory = Path.Combine(trainer.trainingResultsDirectory, trainer.model.checkpoint_settings.run_id);
				trainer.RunTest();
			}
		}
	}
}