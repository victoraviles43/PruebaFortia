using examenTecnicoAnime.Models;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Dynamic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Xml;
using DL;
using Microsoft.EntityFrameworkCore;
using BL;
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
        //string urlApi = "https://cdn.animenewsnetwork.com/encyclopedia/reports.xml?id=177\r\nhttps://cdn.animenewsnetwork.com/encyclopedia/reports.xml?id=155&t\r\nype=anime&nlist=all";
        public async Task<ActionResult> GetAllAsync()
        {
            //Anime = new List<Object>();
            Models.Anime anime = new Models.Anime();
            anime.Animes = new List<Object>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://cdn.animenewsnetwork.com/encyclopedia/reports.xml?id=155&t\r\nype=anime&nlist=50");
                var response = client.GetAsync(client.BaseAddress);
                response.Wait();

                var result = response.Result;

                var readTask = result.Content.ReadAsStringAsync();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(readTask.Result);
                dynamic resultJSON = xmlDocument.DocumentElement;
                //foreach (var resultItem in resultJSON)
                foreach (XmlNode resultItem in xmlDocument.SelectNodes("//item"))
                {
                    examenTecnicoAnime.Models.Anime anime1 = new examenTecnicoAnime.Models.Anime();

                    anime1.Id = resultItem.SelectSingleNode("id").InnerText;
                    anime1.Gid = resultItem.SelectSingleNode("gid").InnerText;
                    anime1.Type = resultItem.SelectSingleNode("type").InnerText;
                    anime1.Name = resultItem.SelectSingleNode("name").InnerText;
                    anime1.Precision = resultItem.SelectSingleNode("precision").InnerText;
                    //anime1.vintage = resultItem.SelectSingleNode("vintage").InnerText;
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
            //return RedirectToAction("Index", "Home");
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
                        ViewBag.Message = "Se ha registrado correctaente el producto";
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
                            //ML.Alumnos alumnos = new ML.Alumnos();
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
                            //ML.Alumnos alumnos = new ML.Alumnos();
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
            return View("GetAllAM",result);
            //return RedirectToAction("GetAllAM");

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
                        result.ErrorMessage = "No se actualizó el status de la credencial";
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
                    client.BaseAddress = new Uri("https://cdn.animenewsnetwork.com/encyclopedia/api.xml?anime=" + "~" + anime.Name);
                    tipo = "anime";
                }
                else
                {
                    client.BaseAddress = new Uri("https://cdn.animenewsnetwork.com/encyclopedia/api.xml?manga=" + "~" + anime.Name);
                    tipo = "manga";
                }

                var response = client.GetAsync(client.BaseAddress);
                response.Wait();

                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsStringAsync();
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(readTask.Result);
                    foreach (XmlNode resultItem in xmlDocument.SelectNodes("//" + tipo))
                    {
                        Models.Anime anime1 = new Models.Anime();

                        anime1.Imagen = resultItem.SelectSingleNode("/ann/" + tipo + "/info[@type='Picture']").Attributes["src"].Value;
                        anime1.Descripcion = resultItem.SelectSingleNode("/ann/" + tipo + "/info[@type='Plot Summary']").InnerText;
                        anime1.Id = resultItem.SelectSingleNode("/ann/" + tipo + "").Attributes["id"].Value;
                        anime1.Name = resultItem.SelectSingleNode("/ann/" + tipo + "").Attributes["name"].Value;
                        anime1.Creador = resultItem.SelectSingleNode("/ann/" + tipo + "/staff/person").InnerText;
                        anime1.Gid = resultItem.SelectSingleNode("/ann/" + tipo + "").Attributes["gid"].Value;
                        anime1.Precision = resultItem.SelectSingleNode("/ann/" + tipo + "").Attributes["precision"].Value;
                        if (resultItem.SelectSingleNode("/ann/" + tipo + "/info[@type='Vintage']") == null)
                        {
                            anime1.Vintage = "";
                        }
                        else
                        {
                            anime1.Vintage = resultItem.SelectSingleNode("/ann/" + tipo + "/info[@type='Vintage']").InnerText;
                        }

                        anime1.Type = resultItem.SelectSingleNode("/ann/" + tipo + "").Attributes["type"].Value;




                        anime.Animes.Add(anime1);
                        anime.Result = true;
                    }
                }
                else
                {
                    return View("Index", anime);
                }
            }
            return View("Index", anime);
        }
    }
}
