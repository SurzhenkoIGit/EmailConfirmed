namespace EmailConfirmed.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string User {  get; set; }
        public string Text { get; set; }
        public DateTime DepartTime { get; set; }
    }
}
