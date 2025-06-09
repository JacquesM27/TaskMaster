namespace TaskMaster.Models.Teaching.School;

public sealed class SchoolMember
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string UniqueNumber { get; set; }
}