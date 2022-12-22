namespace StanDatabase.DTOs
{ 
    public class StudentsPerHouseDTO
    {
        private string _houseName;
        private int _numberOfStudents;

        public string HouseName
        {
            get { return _houseName; }
            set { _houseName = value; }
        }
        public int StudentsCount
        {
            get { return _numberOfStudents; }
            set { _numberOfStudents = value; }
        }
    }
}
