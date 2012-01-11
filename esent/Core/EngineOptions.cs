namespace Meowth.Esentery.Core
{
	/// <summary> Engine start options </summary>
	public struct EngineOptions
	{
		/// <summary> Should this log be circular </summary>
		public bool CircularLog { get; set; }

		public int? MaxVerPages { get; set; }
		public string LogFileDirectory { get; set; }
		public string TempFileDirectory { get; set; }
		public string SystemDirectory { get; set; }
    }
}