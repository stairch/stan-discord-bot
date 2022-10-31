using StanDatabase.Models;

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
    }
}
