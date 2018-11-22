using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using RecordingsDatabase.Helpers;
using RecordingsDatabase.Models;

namespace RecordingsDatabase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordingsController : ControllerBase
    {
        private readonly RecordingsDatabaseContext _context;
        private IConfiguration _configuration;

        public RecordingsController(RecordingsDatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Recordings
        [HttpGet]
        public IEnumerable<Recording> GetRecording()
        {
            return _context.Recording;
        }

        // GET: api/Recordings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecording([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recording = await _context.Recording.FindAsync(id);

            if (recording == null)
            {
                return NotFound();
            }

            return Ok(recording);
        }

        // PUT: api/Recordings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecording([FromRoute] string id, [FromBody] Recording recording)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != recording.Id)
            {
                return BadRequest();
            }

            _context.Entry(recording).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecordingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Recordings
        [HttpPost]
        public async Task<IActionResult> PostRecording([FromBody] Recording recording)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Recording.Add(recording);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecording", new { id = recording.Id }, recording);
        }

        // DELETE: api/Recordings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecording([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recording = await _context.Recording.FindAsync(id);
            if (recording == null)
            {
                return NotFound();
            }

            _context.Recording.Remove(recording);
            await _context.SaveChangesAsync();

            return Ok(recording);
        }

        // GET: api/Recordings/All
        [Route("all")]
        [HttpGet]
        public async Task<List<Recording>> GetAllRecordings()
        {
            var recordings = from r in _context.Recording select r;
            var returned = await recordings.ToListAsync();

            return returned;
        }

        // Get: api/Recordings/search
        [HttpGet]
        [Route("search")]
        public async Task<List<Recording>> GetWordRecordings ([FromQuery] string input)
        {
            var recordings = from r in _context.Recording select r;
            if (!String.IsNullOrEmpty(input)) // Return all recordings if string is empty
            {
                recordings = recordings.Where(s => s.Word.ToLower().Equals(input.ToLower()) || s.Tag.ToLower().Equals(input.ToLower()); // Returns all recordings of the word
            }

            var returned = await recordings.ToListAsync();

            return returned;
        }

        // GET: api/Recordings/tags
        [Route("tag")]
        [HttpGet]
        public async Task<List<string>> GetTags()
        {
            var recordings = (from m in _context.Recording
                         select m.Tag).Distinct();

            var returned = await recordings.ToListAsync();

            return returned;
        }

        private bool RecordingExists(string id)
        {
            return _context.Recording.Any(e => e.Id == id);
        }

        [HttpPost, Route("upload")]
        public async Task<IActionResult> UploadFile([FromForm]RecordingItem recording)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            try
            {
                using (var stream = recording.Image.OpenReadStream())
                {
                    var cloudBlock = await UploadToBlob(recording.Image.FileName, null, stream);
                    //// Retrieve the filename of the file you have uploaded
                    //var filename = provider.FileData.FirstOrDefault()?.LocalFileName;
                    if (string.IsNullOrEmpty(cloudBlock.StorageUri.ToString()))
                    {
                        return BadRequest("An error has occured while uploading your file. Please try again.");
                    }

                    Recording recordingObject = new Recording();
                    recordingObject.Word = recording.Word;
                    recordingObject.Tag = recording.Tag;
                    recordingObject.Syllables = recording.Syllables;
                    recordingObject.Rating = recording.Rating;

                    recordingObject.Url = cloudBlock.SnapshotQualifiedUri.AbsoluteUri;
                    recordingObject.Uploaded = DateTime.Now.ToString();
                    //recordingObject.RecordingLength = image.length;

                    _context.Recording.Add(recordingObject);
                    Console.WriteLine(recordingObject.Id);
                    await _context.SaveChangesAsync();

                    return Ok($"File: {recording.Word} has successfully uploaded");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }


        }

        private async Task<CloudBlockBlob> UploadToBlob(string filename, byte[] imageBuffer = null, System.IO.Stream stream = null)
        {

            var accountName = _configuration["AzureBlob:name"];
            var accountKey = _configuration["AzureBlob:key"]; ;
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer imagesContainer = blobClient.GetContainerReference("recordings");

            string storageConnectionString = _configuration["AzureBlob:connectionString"];

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Generate a new filename for every new blob
                    var fileName = Guid.NewGuid().ToString();
                    fileName += GetFileExtention(filename);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = imagesContainer.GetBlockBlobReference(fileName);

                    if (stream != null)
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return new CloudBlockBlob(new Uri(""));
                    }

                    return cloudBlockBlob;
                }
                catch (StorageException ex)
                {
                    return new CloudBlockBlob(new Uri(""));
                }
            }
            else
            {
                return new CloudBlockBlob(new Uri(""));
            }

        }

        private string GetFileExtention(string fileName)
        {
            if (!fileName.Contains("."))
                return ""; //no extension
            else
            {
                var extentionList = fileName.Split('.');
                return "." + extentionList.Last(); //assumes last item is the extension 
            }
        }

    }
}