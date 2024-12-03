using System.Collections.Generic;
using System.Text;

namespace MachineLearning.Autotrainer
{
	public class Model
	{
		public DefaultSettings default_settings { get; set; } = null;
		public Dictionary<string, Behavior> behaviors { get; set; } = new Dictionary<string, Behavior>
			{
				{
					"MoveToTarget", new Behavior
					{
						trainer_type = "ppo",
						hyperparameters = new Hyperparameters
						{
							batch_size = 4096,
							buffer_size = 16384,
							learning_rate = 0.0003,
							beta = 0.005,
							epsilon = 0.2,
							lambd = 0.95,
							num_epoch = 3,
							learning_rate_schedule = "linear",
							beta_schedule = "linear",
							epsilon_schedule = "linear"
						},
						network_settings = new NetworkSettings
						{
							normalize = false,
							hidden_units = 256,
							num_layers = 3,
							vis_encode_type = "simple",
							memory = null,
							goal_conditioning_type = "hyper",
							deterministic = false
						},
						reward_signals = new Dictionary<string, RewardSignal>
						{
							{
								"extrinsic", new RewardSignal
								{
									gamma = 0.99,
									strength = 1.0,
									network_settings = new NetworkSettings
									{
										normalize = false,
										hidden_units = 512,
										num_layers = 4,
										vis_encode_type = "simple",
										memory = null,
										goal_conditioning_type = "hyper",
										deterministic = false
									}
								}
							}
						},
						init_path = null,
						keep_checkpoints = 5,
						checkpoint_interval = 100000,
						max_steps = 1000000,
						time_horizon = 64,
						summary_freq = 5000,
						threaded = true,
						self_play = null,
						behavioral_cloning = null
					}
				}
			};
		public EnvSettings env_settings { get; set; } = new EnvSettings
		{
			env_path = @"C:\Work\Reflex Aim Trainer\DevBuild\Reflex Aim Trainer.exe",
			env_args = null,
			base_port = 5005,
			num_envs = 1,
			num_areas = 1,
			seed = -1,
			max_lifetime_restarts = 10,
			restarts_rate_limit_n = 1,
			restarts_rate_limit_period_s = 60
		};
		public EngineSettings engine_settings { get; set; } = new EngineSettings
		{
			width = 84,
			height = 84,
			quality_level = 1,
			time_scale = 20,
			target_frame_rate = -1,
			capture_frame_rate = 60,
			no_graphics = true
		};
		public EnvironmentParameters environment_parameters { get; set; } = null;
		public CheckpointSettings checkpoint_settings { get; set; } = new CheckpointSettings
		{
			run_id = "run01",
			initialize_from = null,
			load_model = false,
			resume = false,
			force = false,
			train_model = false,
			inference = false,
			results_dir = "results"
		};
		public TorchSettings torch_settings { get; set; } = new TorchSettings
		{
			device = "cpu"
		};
		public bool debug { get; set; } = false;

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"default_settings: {default_settings?.ToString() ?? "null"}");
			sb.AppendLine("behaviors:");
			foreach (var behavior in behaviors)
			{
				sb.AppendLine($"  {behavior.Key}:");
				sb.AppendLine($"    trainer_type: {behavior.Value.trainer_type}");
				sb.AppendLine("    hyperparameters:");
				sb.AppendLine($"      batch_size: {behavior.Value.hyperparameters.batch_size}");
				sb.AppendLine($"      buffer_size: {behavior.Value.hyperparameters.buffer_size}");
				sb.AppendLine($"      learning_rate: {behavior.Value.hyperparameters.learning_rate}");
				sb.AppendLine($"      beta: {behavior.Value.hyperparameters.beta}");
				sb.AppendLine($"      epsilon: {behavior.Value.hyperparameters.epsilon}");
				sb.AppendLine($"      lambd: {behavior.Value.hyperparameters.lambd}");
				sb.AppendLine($"      num_epoch: {behavior.Value.hyperparameters.num_epoch}");
				sb.AppendLine($"      learning_rate_schedule: {behavior.Value.hyperparameters.learning_rate_schedule}");
				sb.AppendLine($"      beta_schedule: {behavior.Value.hyperparameters.beta_schedule}");
				sb.AppendLine($"      epsilon_schedule: {behavior.Value.hyperparameters.epsilon_schedule}");
				sb.AppendLine("    network_settings:");
				sb.AppendLine($"      normalize: {behavior.Value.network_settings.normalize.ToString().ToLower()}");
				sb.AppendLine($"      hidden_units: {behavior.Value.network_settings.hidden_units}");
				sb.AppendLine($"      num_layers: {behavior.Value.network_settings.num_layers}");
				sb.AppendLine($"      vis_encode_type: {behavior.Value.network_settings.vis_encode_type}");
				sb.AppendLine($"      memory: {behavior.Value.network_settings.memory?.ToString() ?? "null"}");
				sb.AppendLine($"      goal_conditioning_type: {behavior.Value.network_settings.goal_conditioning_type}");
				sb.AppendLine($"      deterministic: {behavior.Value.network_settings.deterministic.ToString().ToLower()}");
				sb.AppendLine("    reward_signals:");
				foreach (var rewardSignal in behavior.Value.reward_signals)
				{
					sb.AppendLine($"      {rewardSignal.Key}:");
					sb.AppendLine($"        gamma: {rewardSignal.Value.gamma}");
					sb.AppendLine($"        strength: {rewardSignal.Value.strength}");
					sb.AppendLine("        network_settings:");
					sb.AppendLine($"          normalize: {rewardSignal.Value.network_settings.normalize.ToString().ToLower()}");
					sb.AppendLine($"          hidden_units: {rewardSignal.Value.network_settings.hidden_units}");
					sb.AppendLine($"          num_layers: {rewardSignal.Value.network_settings.num_layers}");
					sb.AppendLine($"          vis_encode_type: {rewardSignal.Value.network_settings.vis_encode_type}");
					sb.AppendLine($"          memory: {rewardSignal.Value.network_settings.memory?.ToString() ?? "null"}");
					sb.AppendLine($"          goal_conditioning_type: {rewardSignal.Value.network_settings.goal_conditioning_type}");
					sb.AppendLine($"          deterministic: {rewardSignal.Value.network_settings.deterministic.ToString().ToLower()}");
				}
				sb.AppendLine($"    init_path: {behavior.Value.init_path?.ToString() ?? "null"}");
				sb.AppendLine($"    keep_checkpoints: {behavior.Value.keep_checkpoints}");
				sb.AppendLine($"    checkpoint_interval: {behavior.Value.checkpoint_interval}");
				sb.AppendLine($"    max_steps: {behavior.Value.max_steps}");
				sb.AppendLine($"    time_horizon: {behavior.Value.time_horizon}");
				sb.AppendLine($"    summary_freq: {behavior.Value.summary_freq}");
				sb.AppendLine($"    threaded: {behavior.Value.threaded.ToString().ToLower()}");
				sb.AppendLine($"    self_play: {behavior.Value.self_play?.ToString() ?? "null"}");
				sb.AppendLine($"    behavioral_cloning: {behavior.Value.behavioral_cloning?.ToString() ?? "null"}");
			}
			sb.AppendLine("env_settings:");
			sb.AppendLine($"  env_path: {env_settings.env_path}");
			sb.AppendLine($"  env_args: {env_settings.env_args?.ToString() ?? "null"}");
			sb.AppendLine($"  base_port: {env_settings.base_port}");
			sb.AppendLine($"  num_envs: {env_settings.num_envs}");
			sb.AppendLine($"  num_areas: {env_settings.num_areas}");
			sb.AppendLine($"  seed: {env_settings.seed}");
			sb.AppendLine($"  max_lifetime_restarts: {env_settings.max_lifetime_restarts}");
			sb.AppendLine($"  restarts_rate_limit_n: {env_settings.restarts_rate_limit_n}");
			sb.AppendLine($"  restarts_rate_limit_period_s: {env_settings.restarts_rate_limit_period_s}");
			sb.AppendLine("engine_settings:");
			sb.AppendLine($"  width: {engine_settings.width}");
			sb.AppendLine($"  height: {engine_settings.height}");
			sb.AppendLine($"  quality_level: {engine_settings.quality_level}");
			sb.AppendLine($"  time_scale: {engine_settings.time_scale}");
			sb.AppendLine($"  target_frame_rate: {engine_settings.target_frame_rate}");
			sb.AppendLine($"  capture_frame_rate: {engine_settings.capture_frame_rate}");
			sb.AppendLine($"  no_graphics: {engine_settings.no_graphics.ToString().ToLower()}");
			sb.AppendLine($"environment_parameters: {environment_parameters?.ToString() ?? "null"}");
			sb.AppendLine("checkpoint_settings:");
			sb.AppendLine($"  run_id: {checkpoint_settings.run_id}");
			sb.AppendLine($"  initialize_from: {checkpoint_settings.initialize_from?.ToString() ?? "null"}");
			sb.AppendLine($"  load_model: {checkpoint_settings.load_model.ToString().ToLower()}");
			sb.AppendLine($"  resume: {checkpoint_settings.resume.ToString().ToLower()}");
			sb.AppendLine($"  force: {checkpoint_settings.force.ToString().ToLower()}");
			sb.AppendLine($"  train_model: {checkpoint_settings.train_model.ToString().ToLower()}");
			sb.AppendLine($"  inference: {checkpoint_settings.inference.ToString().ToLower()}");
			sb.AppendLine($"  results_dir: {checkpoint_settings.results_dir}");
			sb.AppendLine("torch_settings:");
			sb.AppendLine($"  device: {torch_settings.device}");
			sb.AppendLine($"debug: {debug.ToString().ToLower()}");

			return sb.Replace(',','.').ToString();
		}

		public void WriteToFile(string filePath)
		{
			File.WriteAllText( filePath, this.ToString());
		}
	}

	public class DefaultSettings { }

	public class Behavior
	{
		public string trainer_type { get; set; }
		public Hyperparameters hyperparameters { get; set; }
		public NetworkSettings network_settings { get; set; }
		public Dictionary<string, RewardSignal> reward_signals { get; set; }
		public string init_path { get; set; }
		public int keep_checkpoints { get; set; }
		public int checkpoint_interval { get; set; }
		public int max_steps { get; set; }
		public int time_horizon { get; set; }
		public int summary_freq { get; set; }
		public bool threaded { get; set; }
		public object self_play { get; set; }
		public object behavioral_cloning { get; set; }
	}

	public class Hyperparameters
	{
		public int batch_size { get; set; }
		public int buffer_size { get; set; }
		public double learning_rate { get; set; }
		public double beta { get; set; }
		public double epsilon { get; set; }
		public double lambd { get; set; }
		public int num_epoch { get; set; }
		public string learning_rate_schedule { get; set; }
		public string beta_schedule { get; set; }
		public string epsilon_schedule { get; set; }
	}

	public class NetworkSettings
	{
		public bool normalize { get; set; }
		public int hidden_units { get; set; }
		public int num_layers { get; set; }
		public string vis_encode_type { get; set; }
		public object memory { get; set; }
		public string goal_conditioning_type { get; set; }
		public bool deterministic { get; set; }
	}

	public class RewardSignal
	{
		public double gamma { get; set; }
		public double strength { get; set; }
		public NetworkSettings network_settings { get; set; }
	}

	public class EnvSettings
	{
		public string env_path { get; set; }
		public object env_args { get; set; }
		public int base_port { get; set; }
		public int num_envs { get; set; }
		public int num_areas { get; set; }
		public int seed { get; set; }
		public int max_lifetime_restarts { get; set; }
		public int restarts_rate_limit_n { get; set; }
		public int restarts_rate_limit_period_s { get; set; }
	}

	public class EngineSettings
	{
		public int width { get; set; }
		public int height { get; set; }
		public int quality_level { get; set; }
		public int time_scale { get; set; }
		public int target_frame_rate { get; set; }
		public int capture_frame_rate { get; set; }
		public bool no_graphics { get; set; }
	}

	public class EnvironmentParameters { }

	public class CheckpointSettings
	{
		public string run_id { get; set; }
		public object initialize_from { get; set; }
		public bool load_model { get; set; }
		public bool resume { get; set; }
		public bool force { get; set; }
		public bool train_model { get; set; }
		public bool inference { get; set; }
		public string results_dir { get; set; }
	}

	public class TorchSettings
	{
		public string device { get; set; }
	}


}
