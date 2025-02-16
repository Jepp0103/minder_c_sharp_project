﻿using Microsoft.AspNetCore.Mvc;
using MinderApi.Models;
using MinderApi.Models.Database;
using MySql.Data.MySqlClient;

namespace MinderApi.Controllers
{
    [Produces("application/json")]
    [Route("api/artists")]
    [ApiController]
    public class ArtistController : ControllerBase
    {

        private IConfiguration Configuration;
        private MusicDatabaseMySQL musicDatabase;

        public ArtistController(IConfiguration configuration)
        {
            Configuration = configuration;
            musicDatabase = new MusicDatabaseMySQL(Configuration);
        }

        [HttpGet]
        public JsonResult GetArtists()
        {
            string query = "SELECT * FROM `artist` ORDER BY `Name`;";
            var artistTable = musicDatabase.SelectSQLQuery(query);
            return new JsonResult(artistTable);
        }

        [Route("{artistId}")]
        [HttpGet]
        public JsonResult GetArtistById(int artistId)
        {
            string query = @"SELECT * FROM `artist`
                             WHERE `ArtistId` = @ArtistId;";
            var artistTable = musicDatabase.SelectSQLQueryById(query, "@ArtistId", artistId);
            return new JsonResult(artistTable);
        }



        [Route("search")]
        [HttpGet]
        public JsonResult SearchArtists([FromQuery] string searchString)
        {
            try
            {
                string query = @"
                            SELECT * FROM `artist` 
                            WHERE `Name` LIKE @SearchWord;
                ";
                var artistTable = musicDatabase.SearchSQLQuery(query, searchString);
                return new JsonResult(artistTable);
            }
            catch (MySqlException e){
                return new JsonResult($"Error searching for artists: {e}");    
            }
            
        }

        [HttpPost]
        public JsonResult AddArtist([FromBody] Artist artistModel)
        {
            string insertionQuery = @"
                INSERT INTO `artist` (`Name`)
                VALUES(@Name);
            ";

            var response = musicDatabase.InsertOrUpdateSQLQuery(insertionQuery, new[]
                {
                    new MySqlParameter("@Name", artistModel.Name)
                },
                "artist",
                "ArtistId"
            );

            return new JsonResult(response);
        }

        [Route("{artistId}")]
        [HttpPut]
        public JsonResult UpdateArtist([FromBody] Artist artistModel, int artistId)
        {
            string updateQuery = @$"
                UPDATE `artist` 
                SET `Name` = @Name
                WHERE `ArtistID` = @ArtistId
                ;
            ";

            var response = musicDatabase.InsertOrUpdateSQLQuery(updateQuery, new[]
                {
                    new MySqlParameter("@Name", artistModel.Name),
                    new MySqlParameter("@ArtistId", artistId)
                },
                "artist",
                "ArtistId",
                artistId
            );

            return new JsonResult(response);
        }

        [Route("{artistId}")]
        [HttpDelete]
        public JsonResult DeleteArtist(int artistId)
        {
            try
            {
                string deleteQuery = @$"
                DELETE FROM `artist` 
                WHERE ArtistId = @ArtistId
                ;
                ";

                var response = musicDatabase.DeleteQuery(deleteQuery, artistId, "ArtistId", "artist");
                return new JsonResult(response);
            } catch (MySqlException e) {   
                Console.WriteLine(e.Message);
                return new JsonResult("Error: The artist may have child tables such as albums or tracks and cannot be deleted.");
            }

        }
    }
}
