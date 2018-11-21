using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RecordingsDatabase.Models
{
    public class RecordingsDatabaseContext : DbContext
    {
        public RecordingsDatabaseContext (DbContextOptions<RecordingsDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<RecordingsDatabase.Models.Recording> Recording { get; set; }
    }
}
