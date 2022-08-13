namespace KTech.Notifications.Data.DTOs
{
    public class EmailData
    {
        public string From { get; set; }
        public string[] To { get; set; }
        public string[]? Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
