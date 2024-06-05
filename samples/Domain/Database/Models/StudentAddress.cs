using Domain.Database.Models.Abstracts;

namespace Domain.Database.Models;

public class StudentAddress : AddressBase
{
    // Properties
    public Guid StudentId { get; set; }
    public virtual Student? Student { get; set; }
}
