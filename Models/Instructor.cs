namespace Assignment_3.Models
{
    internal class Instructor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Bio { get; set; }
        public DateTime JoinDate { get; set; }

        public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
    }
}
