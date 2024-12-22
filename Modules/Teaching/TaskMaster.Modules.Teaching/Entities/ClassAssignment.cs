namespace TaskMaster.Modules.Teaching.Entities;

public class ClassAssignment
{
    public Guid Id { get; set; }
    
    public Guid TeachingClassId { get; set; }
    public TeachingClass TeachingClass { get; set; }
    
    public Guid AssignmentId { get; set; }
    public Assignment Assignment { get; set; }
}