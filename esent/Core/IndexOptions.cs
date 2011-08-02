using System;

namespace Meowth.Esentery.Core
{
    /// <summary> Options for index creation and editing </summary>
    public class IndexOptions
    {
        /// <summary> Is index primary or not </summary>
        public bool IsPrimary { get; set; }

        /// <summary> Is index unique or not </summary>
        public bool Unique { get; set; }
        
        /// <summary> Validates settings  </summary>
        internal void Validate()
        {
            throw new NotSupportedException();
        }

        /// <summary> Is index scending or descending </summary>
        public bool IsAscending { get; set; }
    }
}