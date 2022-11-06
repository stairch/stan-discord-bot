using LinqToDB.Mapping;

namespace StanDatabase.Models
{
    [Table(Name = "Students")]
    public class Student
    {
        [PrimaryKey, Identity]
        public int StudentId { get; set; }

        [Column, NotNull]
        public string StudentEmail { get; set; }

        [Column, NotNull]
        public int FkHouseId { get; set; }

        [Association(ThisKey = nameof(FkHouseId), OtherKey = nameof(Models.House.HouseId))]
        public House House { get; set; }

        [Column, NotNull]
        public bool StillStudying { get; set; }

        // can be null because new ex students on the server don't have a semester
        [Column]
        public int Semester { get; set; }

        [Column, NotNull]
        public bool IsDiscordAdmin { get; set; }

        public Student(string studentEmail, int houseId, bool stillStudying, int semester)
        {
            StudentEmail = studentEmail;
            House = House.GetHouseById(houseId);
            StillStudying = stillStudying;
            Semester = semester;
        }

        public override string ToString()
        {
            return $"{base.ToString()}[" +
                $"{nameof(StudentId)}: {StudentId}, " +
                $"{nameof(StudentEmail)}: {StudentEmail}, " +
                $"{nameof(FkHouseId)}: {FkHouseId}, " +
                $"{nameof(StillStudying)}: {StillStudying}, " +
                $"{nameof(Semester)}: {Semester}" +
                $"]";
        }
    }
}
