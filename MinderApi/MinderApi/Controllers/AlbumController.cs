﻿using System.Data;
using Microsoft.AspNetCore.Mvc;
using MinderApi.Models;
using MinderApi.Models.Database;
using Microsoft.EntityFrameworkCore;


namespace MinderApi.Controllers
{
    [Produces("application/json")]
    [Route("api/albums")]
    [ApiController]
    public class AlbumController : Controller
    {
        private readonly MusicDatabaseEFContext musicDbContext;

        public AlbumController(MusicDatabaseEFContext mDbContext)
        {
            musicDbContext = mDbContext;
        }

        [HttpGet]
        public IEnumerable<Album> GetAlbums() {
            var albums = musicDbContext.Album.ToList();
            return albums;
        }

        [Route("{albumId}")]
        [HttpGet]
        public IEnumerable<Album> GetAlbumById(int albumId) {
            var album = musicDbContext.Album.Where(album => album.AlbumId == albumId);
            System.Diagnostics.Debug.WriteLine("album" + album);

            return album;
        }

        [Route("search")]
        [HttpGet]
        public IEnumerable<Album> SearchAlbums([FromQuery] string searchString) {
            var albums = musicDbContext.Album.Where(album => album.Title.Contains(searchString));
            return albums;
        }

        [HttpPost]
        public IActionResult AddAlbum([FromBody] Album newAlbum) {
            try {
                musicDbContext.Album.Add(newAlbum);
                musicDbContext.SaveChanges();

                var insertedAlbum = musicDbContext.Album.Where(album => album.AlbumId == newAlbum.AlbumId);
                return new JsonResult(insertedAlbum);
            } catch (Exception e) {
                return new JsonResult(e);
            }
        }

        [HttpPut]
        [Route("{albumId}")]
        public IActionResult UpdateAlbum([FromBody] Album albumModel, int albumId) {
            try {
                Album albumToUpdate = musicDbContext.Album.Single(album => album.AlbumId == albumId);
                albumToUpdate.Title = albumModel.Title;
                albumToUpdate.ArtistId = albumModel.ArtistId;
                musicDbContext.SaveChanges();

                var updatedAlbum = musicDbContext.Album.Where(album => album.AlbumId == albumId);
                return new JsonResult(updatedAlbum);

            } catch (DbUpdateException e) {
                return new JsonResult($"An error occured while updating the album. Update of child records is not possible, " +
                    $" which may be the problem: {e}");
            }
        }

        [HttpDelete]
        [Route("{albumId}")]
        public IActionResult DeleteAlbum(int albumId) {
            try {
                Album albumToDelete = musicDbContext.Album.Single(album => album.AlbumId == albumId);
                musicDbContext.Album.Remove(albumToDelete);
                musicDbContext.SaveChanges();
                return new JsonResult($"Album deleted with id: {albumId}");

            } catch (Exception e) {
                return new JsonResult($"Album cannot be deleted, since the album contains child records: {e}");
            }
        }
    }
}
