namespace APIHungryHunters.Models
{
    public class TodasEmpresasDTO
    {
        public string Nipc { get; set; }
        public string Razao_social { get; set; }
        public string Email { get; set; }
        public int Num_Restaurante { get; set; }
        public string Telemovel { get; set; }
        public string Password { get; set; }
        public List<Restaurantes> Restaurantes { get; set; }
    }
}
