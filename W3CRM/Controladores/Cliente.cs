namespace W3CRM.Controllers
{
    public class Cliente
    {
        public int ID { get; set; }
        public string Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string Telefono { get; set; }
        public int? Edad { get; set; }
        public string? Genero { get; set; }
        public DateTime? Ultima_Conusulta { get; set; }
        public DateTime Ingreso { get; set; }
    }
}
