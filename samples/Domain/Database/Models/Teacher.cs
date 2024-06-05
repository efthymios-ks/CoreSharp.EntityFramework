using CoreSharp.EntityFramework.Entities.Abstracts;

namespace Domain.Database.Models;

public class Teacher : EntityBase<Guid>
{
    // Properties
    public string? Name { get; set; }
    public TeacherType TeacherType { get; set; }
    public virtual ICollection<Course> Courses { get; set; } = [];
}
