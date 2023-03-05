using examenTecnicoAnime.Models;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System;
using System.Text;
using System.Xml.Serialization;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Xml;
using DL;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace examenTecnicoAnime.Controllers
{
    public class AnimeController : Controller


    {
        private readonly IConfiguration _configuration;

        private readonly IHostingEnvironment _hostingEnvironment;

        public AnimeController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }
      
        public async Task<ActionResult> GetAllAsync()
        {
            
            Models.Anime anime = new Models.Anime();
            anime.Animes = new List<Object>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration["WebAPI"]);
                var response = client.GetAsync(client.BaseAddress);
                response.Wait();

                var result = response.Result;

                var readTask = result.Content.ReadAsStringAsync();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(readTask.Result);

               
                foreach (XmlNode resultItem in xmlDocument.SelectNodes("//item"))
                {
                    examenTecnicoAnime.Models.Anime anime1 = new examenTecnicoAnime.Models.Anime();

                    anime1.Id = resultItem.SelectSingleNode("id").InnerText;
                    anime1.Gid = resultItem.SelectSingleNode("gid").InnerText;
                    anime1.Type = resultItem.SelectSingleNode("type").InnerText;
                    anime1.Name = resultItem.SelectSingleNode("name").InnerText;
                    anime1.Precision = resultItem.SelectSingleNode("precision").InnerText;
                
                    if (resultItem.SelectSingleNode("vintage") == null)
                    {
                        anime.Vintage = "";

                    }

                    else
                    {
                        anime1.Vintage = resultItem.SelectSingleNode("vintage").InnerText;
                    }


                    anime.Animes.Add(anime1);


                }


            }
            return View("Index", anime);
           
        }
        [HttpGet]
        public ActionResult Add(Models.Anime anime)
        {
            Result result = new Result();
            try
            {
                using (DL.AnimeContext context = new DL.AnimeContext())
                {
                    int query = 0;
                    if (anime.Precision == "manga")
                    {
                        query = context.Database.ExecuteSqlRaw($"MangaAdd '{anime.Id}', '{anime.Gid}', '{anime.Type}', '{anime.Name}', '{anime.Precision}', '´{anime.Vintage}'");
                    }
                    else
                    {
                        query = context.Database.ExecuteSqlRaw($"AnimeAdd '{anime.Id}', '{anime.Gid}', '{anime.Type}', '{anime.Name}', '{anime.Precision}', '´{anime.Vintage}'");
                    }

                    if (query > 0)
                    {
                        ViewBag.Message = "Se ha registrado correctaente el elemento";
                        return PartialView("Modal");
                    }
                    else
                    {
                        ViewBag.Message = "Ha ocurrido un error" + result.ErrorMessage;
                        return PartialView("Modal");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;
                result.Ex = ex;
            }
            return PartialView("Modal");
        }
        [HttpGet]
        public ActionResult GetAllAM()

        {
            Result result = new Result();
            result.anime = new Models.Anime();
            try
            {
                using (DL.AnimeContext context = new DL.AnimeContext())
                {
                    var query = context.Animes.FromSqlRaw("AnimeGetAll").ToList();
                    var querymanga = context.Mangas.FromSqlRaw("MangaGetAll").ToList();

                    result.anime.Animes = new List<object>();
                    result.anime.Mangas = new List<object>();

                    if (query != null)
                    {
                        foreach (var obj in query)
                        {
                            
                            examenTecnicoAnime.Models.Anime animes = new examenTecnicoAnime.Models.Anime();


                            animes.Id = obj.Id;
                            animes.Gid = obj.Gid;
                            animes.Type = obj.Type;
                            animes.Name = obj.Name;
                            animes.Precision = obj.Precision;
                            animes.Vintage = obj.Vintage;

                            result.anime.Animes.Add(animes);
                        }
                        result.Correct = true;
                    }


                    if (querymanga != null)
                    {
                        foreach (var obj in querymanga)
                        {
                            
                            examenTecnicoAnime.Models.Anime mangas = new examenTecnicoAnime.Models.Anime();


                            mangas.Id = obj.Id;
                            mangas.Gid = obj.Gid;
                            mangas.Type = obj.Type;
                            mangas.Name = obj.Name;
                            mangas.Precision = obj.Precision;
                            mangas.Vintage = obj.Vintage;

                            result.anime.Mangas.Add(mangas);
                        }
                        result.Correct = true;
                    }

                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se encontraron registros";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;
                result.Ex = ex;
            }
            return View("GetAllAM", result);
            

        }

        [HttpGet]

        public ActionResult Delete(Models.Anime anime)
        {
            Result result = new Result();
            result.anime = new Models.Anime();
            try
            {
                using (DL.AnimeContext context = new DL.AnimeContext())
                {
                    int query = 0;
                    query = context.Database.ExecuteSqlRaw($"DeleteManga '{anime.Id}'");


                    if (query >= 1)
                    {
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se elimino";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;
                result.Ex = ex;
            }
            return PartialView("Modal");

        }

        public ActionResult Busqueda(Models.Anime anime)
        {
            anime.Animes = new List<Object>();

            using (var client = new HttpClient())
            {
                string tipo = "";
                if (anime.Descripcion == "anime")
                {
                    client.BaseAddress = new Uri(_configuration["WebAPIAnime"] + "~" + anime.Name);
                    tipo = "anime";
                }
                else
                {
                    client.BaseAddress = new Uri(_configuration["WebAPIManga"] + "~" + anime.Name);
                    tipo = "manga";
                }

                using (var response = client.GetAsync(client.BaseAddress))
                {
                    response.Wait();

                    var result = response.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        var readTask = result.Content.ReadAsStringAsync();
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(readTask.Result);
                        var prueba = xmlDocument.SelectNodes("//" + tipo);
                        foreach (XmlNode resultItem in prueba)
                        {
                            Models.Anime anime1 = new Models.Anime();

                            anime1.Gid = resultItem.Attributes["gid"].Value;
                            anime1.Gid = resultItem.Attributes["type"].Value;
                            anime1.Name = resultItem.Attributes["name"].Value;
                            anime1.Precision = resultItem.Attributes["precision"].Value;
                            anime1.Creador = resultItem.SelectSingleNode("staff/person").InnerText;
                            anime1.Id = resultItem.Attributes["id"].Value;
                            if (resultItem.SelectSingleNode("info[@type='Plot Summary']") == null)
                            {
                                anime1.Descripcion = tipo + " Sin Descripción";

                            }

                            else
                            {
                                anime1.Descripcion = resultItem.SelectSingleNode("info[@type='Plot Summary']").InnerText;
                            }
                            if (resultItem.SelectSingleNode("info[@type = 'Picture'] / img / @src") == null)
                            {
                                anime1.Imagen = "https://islandpress.org/sites/default/files/default_book_cover_2015.jpg";
                            }
                            else
                            {
                                anime1.Imagen = resultItem.SelectSingleNode("info[@type = 'Picture'] / img / @src").Value;
                            }

                            anime.Animes.Add(anime1);
                            anime.Result = true;
                        }
                    }
                    else
                    {
                        return View("Index", anime);
                    }
                }
            }
            return View("Index", anime);
        }
    }
}
