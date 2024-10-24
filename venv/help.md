positional arguments:
  trainer_config_path

optional arguments:
  -h, --help            show this help message and exit
  --env ENV_PATH        Path to the Unity executable to train (default: None)
  --resume              Whether to resume training from a checkpoint. Specify a --run-id to use this option. If set, the training code loads an already trained model to initialize the neural network before resuming training. This option is only
                        valid when the models exist, and have the same behavior names as the current agents in your scene. (default: False)
  --deterministic       Whether to select actions deterministically in policy. `dist.mean` for continuous action space, and `dist.argmax` for deterministic action space (default: False)
  --force               Whether to force-overwrite this run-id's existing summary and model data. (Without this flag, attempting to train a model with a run-id that has been used before will throw an error. (default: False)
  --run-id RUN_ID       The identifier for the training run. This identifier is used to name the subdirectories in which the trained model and summary statistics are saved as well as the saved model itself. If you use TensorBoard to view the
                        training statistics, always set a unique run-id for each training run. (The statistics for all runs with the same id are combined as if they were produced by a the same session.) (default: ppo)
  --initialize-from RUN_ID
                        Specify a previously saved run ID from which to initialize the model from. This can be used, for instance, to fine-tune an existing model on a new environment. Note that the previously saved models must have the same
                        behavior parameters as your current environment. (default: None)
  --seed SEED           A number to use as a seed for the random number generator used by the training code (default: -1)
  --inference           Whether to run in Python inference mode (i.e. no training). Use with --resume to load a model trained with an existing run ID. (default: False)
  --base-port BASE_PORT
                        The starting port for environment communication. Each concurrent Unity environment instance will get assigned a port sequentially, starting from the base-port. Each instance will use the port (base_port + worker_id), where
                        the worker_id is sequential IDs given to each instance from 0 to (num_envs - 1). Note that when training using the Editor rather than an executable, the base port will be ignored. (default: 5005)
  --num-envs NUM_ENVS   The number of concurrent Unity environment instances to collect experiences from when training (default: 1)
  --num-areas NUM_AREAS
                        The number of parallel training areas in each Unity environment instance. (default: 1)
  --debug               Whether to enable debug-level logging for some parts of the code (default: False)
  --env-args ...        Arguments passed to the Unity executable. Be aware that the standalone build will also process these as Unity Command Line Arguments. You should choose different argument names if you want to create environment-specific
                        arguments. All arguments after this flag will be passed to the executable. (default: None)
  --max-lifetime-restarts MAX_LIFETIME_RESTARTS
                        The max number of times a single Unity executable can crash over its lifetime before ml-agents exits. Can be set to -1 if no limit is desired. (default: 10)
  --restarts-rate-limit-n RESTARTS_RATE_LIMIT_N
                        The maximum number of times a single Unity executable can crash over a period of time (period set in restarts-rate-limit-period-s). Can be set to -1 to not use rate limiting with restarts. (default: 1)
  --restarts-rate-limit-period-s RESTARTS_RATE_LIMIT_PERIOD_S
                        The period of time --restarts-rate-limit-n applies to. (default: 60)
  --torch               (Removed) Use the PyTorch framework. (default: False)
  --tensorflow          (Removed) Use the TensorFlow framework. (default: False)
  --results-dir RESULTS_DIR
                        Results base directory (default: results)

Engine Configuration:
  --width WIDTH         The width of the executable window of the environment(s) in pixels (ignored for editor training). (default: 84)
  --height HEIGHT       The height of the executable window of the environment(s) in pixels (ignored for editor training) (default: 84)
  --quality-level QUALITY_LEVEL
                        The quality level of the environment(s). Equivalent to calling QualitySettings.SetQualityLevel in Unity. (default: 5)
  --time-scale TIME_SCALE
                        The time scale of the Unity environment(s). Equivalent to setting Time.timeScale in Unity. (default: 20)
  --target-frame-rate TARGET_FRAME_RATE
                        The target frame rate of the Unity environment(s). Equivalent to setting Application.targetFrameRate in Unity. (default: -1)
  --capture-frame-rate CAPTURE_FRAME_RATE
                        The capture frame rate of the Unity environment(s). Equivalent to setting Time.captureFramerate in Unity. (default: 60)
  --no-graphics         Whether to run the Unity executable in no-graphics mode (i.e. without initializing the graphics driver. Use this only if your agents don't use visual observations. (default: False)

Torch Configuration:
  --torch-device DEVICE
                        Settings for the default torch.device used in training, for example, "cpu", "cuda", or "cuda:0" (default: None)