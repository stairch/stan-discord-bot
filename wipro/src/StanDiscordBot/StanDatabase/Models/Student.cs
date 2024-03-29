﻿using LinqToDB.Mapping;

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

        /// <summary>
        /// can be null because new ex students on the server don't have a semester
        /// </summary>
        [Column]
        public int Semester { get; set; }

        /// <summary>
        /// Needed but not for Discord permissions (use roles on Discord server for this instead).
        /// Instead it is used to inform the student about issues with the Stan Discord bot per email.
        /// </summary>
        [Column, NotNull]
        public bool IsDiscordAdmin { get; set; }

        public static Student CreateNew(string studentEmail, House house, bool stillStudying, int semester)
        {
            Student student = new Student();
            student.StudentEmail = studentEmail;
            student.House = house;
            student.FkHouseId = house.HouseId;
            student.StillStudying = stillStudying;
            student.Semester = semester;
            return student;
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
