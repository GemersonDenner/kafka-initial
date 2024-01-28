namespace Consumer;

public class Message
{
    public Message(string text)
    {
        this.Id = Guid.NewGuid();
        this.Text = text;
        this.CreatedDate = DateTime.UtcNow;
    }
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedDate { get; private set; }
}