using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordingsDatabase.Models
{
    public class Recording
    {
        public string Id { get; set; }
        public string Word { get; set; }
        public string Syllables { get; set; }
        public string Url { get; set; }
        public string Tag { get; set; }
        public string Uploaded { get; set; }
        //public string RecordingLength { get; set; }
        public string Rating { get; set; }
    }
}
