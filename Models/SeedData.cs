using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordingsDatabase.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new RecordingsDatabaseContext(
                serviceProvider.GetRequiredService<DbContextOptions<RecordingsDatabaseContext>>()))
            {
                // Look for any movies.
                if (context.Recording.Count() > 0)
                {
                    return;   // DB has been seeded
                }

                context.Recording.AddRange(
                    new Recording
                    {
                        Word = "George",
                        Syllables = "George",
                        Url = "Recording.mp4",
                        Tag = "Name",
                        Uploaded = "07-10-18 4:20T18:25:43.511Z",
                        //RecordingLength = "5",
                        Rating = "true"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
