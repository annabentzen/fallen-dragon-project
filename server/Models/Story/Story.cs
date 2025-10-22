public class Story
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<Act> Acts { get; set; }
}

public class Act
{
    public int Id { get; set; }
    public string Text { get; set; }
    public List<Choice> Choices { get; set; }
}

public class Choice
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int NextActId { get; set; }  // links to next act
}
