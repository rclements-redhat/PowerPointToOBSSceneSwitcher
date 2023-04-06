
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPointToOBSSceneSwitcher
{

	public class ObsLocal : IDisposable
	{
		private bool _DisposedValue;
		private OBSWebsocketDotNet.OBSWebsocket _OBS;
		private List<string> validScenes;
		private string defaultScene;

		public ObsLocal() { }

		public Task Connect()
		{
			_OBS = new OBSWebsocketDotNet.OBSWebsocket();
			_OBS.ConnectAsync($"ws://192.168.0.120:4455", "");
			return Task.CompletedTask;
		}

		public string DefaultScene
        {
            get { return defaultScene; }
			set
			{
				if (validScenes.Contains(value))
				{
					defaultScene = value;
				}
                else
                {
                    Console.WriteLine($"Scene named {value} does not exist and cannot be set as default");
                }
			}
        }

		public bool ChangeScene(string scene)
        {
			if (!validScenes.Contains(scene))
			{
                Console.WriteLine($"Scene named {scene} does not exist");
				if (String.IsNullOrEmpty(defaultScene))
				{
                    Console.WriteLine("No default scene has been set!");
					return false;
				}
			
				scene = defaultScene;
			}
			_OBS.SetCurrentProgramScene(scene);


			return true;
        }

		public void GetScenes()
        {
			var allScene = _OBS.GetSceneList();
			var list = allScene.Scenes.Select(s => s.Name).ToList();
			// Console.WriteLine("Valid Scenes:");
			foreach(var l in list)
            {
                Console.WriteLine(l);
            }
			validScenes = list;
        }

		public bool StartRecording()
		{
			try { _OBS.StartRecord(); }
			catch {  /* Recording already started */ }
			return true;
		}

		public bool StopRecording()
		{
			try { _OBS.StopRecord(); }
			catch {  /* Recording already stopped */ }
			return true;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_DisposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
				}

				_OBS.Disconnect();
				_OBS = null;
				_DisposedValue = true;
			}
		}

		~ObsLocal()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}