using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Music_Extract_Feature;
using MusicIdentifierAPI.Models;
using MusicIdentifierAPI.Services;

namespace MusicIdentifierAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : Controller
    {
        private readonly ISongService _songService;
        public SongController(ISongService songService)
        {
            _songService = songService;
        }

        [HttpPost]
        [Route("analyzeData")]
        public ActionResult<SongInfoModel> SongAnalyze([FromBody] File file)
        {
            if (file.ContentType != "audio/wav" && file.ContentType != "audio/wave")
                return BadRequest();
            var song = _songService.AnalyzeAudioFile(Convert.FromBase64String(file.Content));
            if (song == null)
                return NotFound("Song not found");
            return song;
        }

        [HttpPost]
        [Route("analyze")]
        public ActionResult<SongInfoModel> SongAnalyze([FromForm] IFormFile file)
        {
            if (file.ContentType != "audio/wav" && file.ContentType != "audio/wave" || file.Length < 1)
                return BadRequest();
            var song = _songService.AnalyzeAudioFile(file.GetBytes());
            if (song == null)
                return NotFound("Song not found");
            return song;
        }
    }
    public class File
    {
        public string Content { get; set; }
        public string ContentType { get; set; }
    }
}