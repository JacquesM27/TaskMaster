﻿namespace TaskMaster.Modules.Teaching.Entities;

public class TeachingClass
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Level { get; set; }
    public string Language { get; set; }
    public Guid MainTeacherId { get; set; }
    public IList<Guid> SubTeachersIds { get; set; }
    public IList<Guid> StudentsIds { get; set; }
    
    public Guid SchoolId { get; set; }
    public School School { get; set; }
    
    public IList<ClassAssignment> Assignments { get; set; }
}