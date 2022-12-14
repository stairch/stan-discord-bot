using StanDatabase.Models;
using StanDatabase.DTOs;

namespace StanDatabase.Repositories
{
    public interface IStudentRepository
    {
        void InsertMultiple(IList<Student> students);

        /// <summary>
        /// Deactivates all students that aren't in the given list.
        /// Means, StillStudying is set to false, for the student.
        /// </summary>
        /// <param name="students"></param>
        void DeactivateOldStudents(IList<Student> students);

        /// <summary>
        /// Checks if a student with the given E-Mail exists.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Returns the student found or null otherwise.</returns>
        Student? FindWithEmail(string email);

        /// <summary>
        /// Returns the number of students for each house.
        /// </summary>
        /// <returns>List of StudentsPerHouseDTO.</returns>
        public List<StudentsPerHouseDTO> NumberOfStudentsPerHouse();

        /// <summary>
        /// Returns the number of students for each semester.
        /// </summary>
        /// <returns>List of StudentsPerSemesterDTO.</returns>
        public List<StudentsPerSemesterDTO> NumberOfStudentsPerSemester();

        /// Gets the students, which are still studying.
        /// </summary>
        /// <returns></returns>
        List<Student> GetCurrentStudents();

        /// <summary>
        /// Sets the isDiscordAdmin field for the given student.
        /// </summary>
        /// <param name="student"></param>
        /// <param name="isAdmin"></param>
        void SetStudentIsAdmin(Student student, bool isAdmin);

        /// <summary>
        /// Returns all Students, which are marked as DiscordAdmins.
        /// </summary>
        /// <returns></returns>
        IList<Student> GetAllDiscordAdmins();
    }
}
