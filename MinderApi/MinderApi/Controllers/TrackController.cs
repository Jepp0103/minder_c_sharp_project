﻿using Microsoft.AspNetCore.Mvc;
using MinderApi.Models;
using MinderApi.Models.Database;
using MySql.Data.MySqlClient;

namespace MinderApi.Controllers
{
    [Produces("application/json")]
    [Route("api/tracks")]
    [ApiController]
    public class TrackController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly MusicDatabaseMySQL musicDatabase;

        public TrackController(IConfiguration configuration)
        {
            Configuration = configuration;
            musicDatabase = new MusicDatabaseMySQL(Configuration);
        }

        [HttpGet]
        public JsonResult GetTracks() {
            string query = @"   
                            SELECT t.`TrackId`, t.`Name`  as `TrackName`, ar.`Name` as `ArtistName`, al.`Title` as `AlbumTitle`, g.`Name` as `GenreName`, t.`Composer`, t.`Milliseconds`, t.`UnitPrice`, t.`Bytes` FROM `track` t
                            INNER JOIN `album` al ON t.`AlbumId` = al.`AlbumId`
                            INNER JOIN `artist`ar ON al.`ArtistId` = ar.`ArtistId`
                            INNER JOIN `genre` g ON t.`GenreId` = g.`GenreId`
                            ORDER BY `TrackName`; ";
            var trackTable = musicDatabase.SelectSQLQuery(query);
            return new JsonResult(trackTable);
        }

        [HttpGet]
        [Route("likes/{customerId}")]
        public JsonResult GetLikedTracksByUserId(int customerId)
        {
            string query = @"   
                            SELECT l.`LikeId`, t.`TrackId`, t.`Name`  as `TrackName`, ar.`Name` as `ArtistName`, al.`Title` as `AlbumTitle`, 
                            g.`Name` as `GenreName`, t.`Composer`, t.`Milliseconds`, t.`UnitPrice`, t.`Bytes` FROM `track` t
                            INNER JOIN `album` al ON t.`AlbumId` = al.`AlbumId`
                            INNER JOIN `artist`ar ON al.`ArtistId` = ar.`ArtistId`
                            INNER JOIN `genre` g ON t.`GenreId` = g.`GenreId`
                            INNER JOIN `like` l ON t.`TrackId` = l.`TrackId`
                            INNER JOIN `customer` c ON c.`CustomerId` = l.`CustomerId`
                            WHERE c.`CustomerId` = @CustomerId
                            ORDER BY `TrackName`; ";

            var trackTable = musicDatabase.SelectSQLQueryById(query, "@CustomerId", customerId);

            return new JsonResult(trackTable);
        }

        [Route("{trackId}")]
        [HttpGet]
        public JsonResult GetTrackById(int trackId)
        {
            string query = @"SELECT * FROM `track`
                             WHERE `TrackId` = @TrackId;";
            var trackTable = musicDatabase.SelectSQLQueryById(query, "@TrackId", trackId);

            return new JsonResult(trackTable);
        }

        [Route("search")]
        [HttpGet]
        public JsonResult SearchTracks([FromQuery] string searchString)
        {
            string query = @"
                            SELECT t.`TrackId`, t.`Name` as `TrackName`, ar.`Name` as `ArtistName`, al.`Title` as `AlbumTitle`, g.`Name`  as `GenreName`, t.`Composer` FROM `track` t
                            INNER JOIN `album` al ON t.`AlbumId` = al.`AlbumId`
                            INNER JOIN `artist`ar ON al.`ArtistId` = ar.`ArtistId`
                            INNER JOIN `genre` g ON t.`GenreId` = g.`GenreId`
                            WHERE t.`Name` LIKE @SearchWord;
            ";
            var trackTable = musicDatabase.SearchSQLQuery(query, searchString);
            return new JsonResult(trackTable);
        }

        [HttpPost]
        public JsonResult AddTrack([FromBody] Track trackModel) {
            string insertionQuery = @"
                INSERT INTO `track` (`Name`, `AlbumId`, `MediaTypeId`, `GenreId`, `Composer`, `Milliseconds`, `Bytes`, `UnitPrice`)
                VALUES(@Name, @AlbumId, @MediaTypeId, @GenreId, @Composer, @Milliseconds, @Bytes, @UnitPrice);
            ";

            var response = musicDatabase.InsertOrUpdateSQLQuery(insertionQuery, new[]
                {
                    new MySqlParameter("@Name", trackModel.Name),
                    new MySqlParameter("@AlbumId", trackModel.AlbumId),
                    new MySqlParameter("@MediaTypeId", trackModel.MediaTypeId),
                    new MySqlParameter("@GenreId", trackModel.GenreId),
                    new MySqlParameter("@Composer", trackModel.Composer),
                    new MySqlParameter("@Milliseconds", trackModel.MilliSeconds),
                    new MySqlParameter("@Bytes", trackModel.Bytes),
                    new MySqlParameter("@UnitPrice", trackModel.UnitPrice)
                },
                "track",
                "TrackId"
            );

            return new JsonResult(response);
        }

        [Route("{trackId}")]
        [HttpPut]
        public JsonResult UpdateTrack([FromBody] Track trackModel, int trackId)
        {
            try
            {
                string updateQuery = @$"
                UPDATE `track` 
                SET `Name` = @Name, `AlbumId` = @AlbumId, `MediaTypeId` = @MediaTypeId, `GenreId` = @GenreId, 
                    `Composer` = @Composer, `Milliseconds` = @Milliseconds, `Bytes` = @Bytes, `UnitPrice` = @UnitPrice
                WHERE `TrackID` = @TrackId
                ;
            ";

                var response = musicDatabase.InsertOrUpdateSQLQuery(updateQuery, new[]
                    {
                    new MySqlParameter("@Name", trackModel.Name),
                    new MySqlParameter("@AlbumId", trackModel.AlbumId),
                    new MySqlParameter("@MediaTypeId", trackModel.MediaTypeId),
                    new MySqlParameter("@GenreId", trackModel.GenreId),
                    new MySqlParameter("@Composer", trackModel.Composer),
                    new MySqlParameter("@Milliseconds", trackModel.MilliSeconds),
                    new MySqlParameter("@Bytes", trackModel.Bytes),
                    new MySqlParameter("@UnitPrice", trackModel.UnitPrice),
                    new MySqlParameter("@TrackId", trackId)

                },
                    "track",
                    "TrackId",
                    trackId
                );

                return new JsonResult(response);

            } catch(Exception e) {
                return new JsonResult($"Something went wrong: {e}");
            }
        }

        [Route("{trackId}")]
        [HttpDelete]
        public JsonResult DeleteTrack(int trackId)
        {
            try
            {
                string deleteQuery = @$"
                DELETE FROM `track` 
                WHERE TrackId = @TrackId
                ;
            ";

                var response = musicDatabase.DeleteQuery(deleteQuery, trackId, "TrackId", "track");
                return new JsonResult(response);

            }
            catch (Exception e)
            {
                return new JsonResult($"Can't delete track: {e}");
            }
        }
    }   
}
