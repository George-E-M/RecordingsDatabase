using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordingsDatabase.Models
{
    public class RecordingItem
    {
        public string Word { get; set; }
        public string Tags { get; set; }
        public string Syllables { get; set; }
        public string Rating { get; set; }
        public IFormFile Image { get; set; }
    }
}
