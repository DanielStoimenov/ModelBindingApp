namespace ModelBinding;

public class Person
{
    public Person(int id, string name, string egn)
    {
        Id = id;
        Name = name;
        Egn = egn;
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Egn { get; set; }
}