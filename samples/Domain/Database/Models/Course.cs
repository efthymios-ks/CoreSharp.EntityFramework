using CoreSharp.EntityFramework.Entities.Abstracts;

namespace Domain.Database.Models;

public class Course : EntityBase<Guid>
{
    // Properties
    public string? Name { get; set; }
    public Guid TeacherId { get; set; }
    public virtual Teacher? Teacher { get; set; }
    public virtual ICollection<Student> Students { get; set; } = [];
    public virtual ICollection<CourseField> Fields { get; set; } = [];
}
