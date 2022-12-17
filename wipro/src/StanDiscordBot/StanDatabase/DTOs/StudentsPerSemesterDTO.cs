namespace StanDatabase.DTOs
{
    public class StudentsPerSemesterDTO
    {
        private int _semester;
        private int _studentsCount;

        public int Semester { 
            get { return _semester; } 
            set { _semester = value; }
        }

        public int StudentsCount { 
            get { return _studentsCount; }
            set { _studentsCount = value; }
        }
    }
}
