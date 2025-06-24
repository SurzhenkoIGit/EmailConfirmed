namespace EmailConfirmed.Models.Client
{
    public class ClientsViewModel
    {
        public List<ClientApp> Clients { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
        public string SelectedClients { get; set; }
        public int PageNumb { get; set; }
        public int PageSize { get; set; }
    }
}
