using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        public IHostingEnvironment HostingEnvironment { get; }
        public SongController(ISongService songService, IHostingEnvironment env)
        {
            _songService = songService;
            HostingEnvironment = env;
        }

        [HttpPost]
        [Route("analyzeData")]
        public ActionResult<SongInfoModel> SongAnalyze([FromBody] File file)
        {
            if (file.ContentType != "audio/wav" && file.ContentType != "audio/wave")
                return BadRequest();
            try
            {
                var song = _songService.AnalyzeAudioFile(Convert.FromBase64String(file.Content),
                    HostingEnvironment.WebRootPath);
                if (song == null)
                    return NotFound("Song not found");
                return song;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
           
        }

        [HttpPost]
        [Route("analyze")]
        public ActionResult<SongInfoModel> SongAnalyze([FromForm] IFormFile file)
        {
            if (file.ContentType != "audio/wav" && file.ContentType != "audio/wave" || file.Length < 1)
                return BadRequest();
            try
            {
                var song = _songService.AnalyzeAudioFile(file.GetBytes(), HostingEnvironment.WebRootPath);
                if (song == null)
                    return NotFound("Song not found");
                return song;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("/api/song/add/playlist")]
        public ActionResult<PlaylistModel> AddPlaylist([FromForm] PlaylistModel playlistModel)
        {
            var result = _songService.AddPlaylist(playlistModel);
            if (result == null)
                return BadRequest();
            return result;
        }

        [HttpPost]
        [Route("/api/song/add/playlist/song")]
        public ActionResult<bool> AddSongToPlaylist([FromForm] SongInPlaylist songInPlaylist)
        {
            var result = _songService.AddSongToPlaylist(songInPlaylist);
            if (result == false)
                return BadRequest();
            return true;
        }
    }
    public class File
    {
        public string Content { get; set; }
        public string ContentType { get; set; }
    }
}