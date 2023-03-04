namespace examenTecnicoAnime.Models
{
    public class Anime
    {

        public string Id { get; set; }
        public string Gid { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Precision { get; set; }
        public string Imagen { get; set; }
        public string Descripcion { get; set; }
        public string Creador { get; set; }
        public bool Result { get; set; }
        public string Vintage { get; set; }
        public List<object> Animes { get; set; }
        public List<object> Mangas { get; set; }                




    }
}
