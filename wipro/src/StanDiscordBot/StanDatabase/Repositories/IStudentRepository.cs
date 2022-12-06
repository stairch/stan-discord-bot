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

        /// <summary>
        /// Checks if a student with the given E-Mail exists.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Returns the student found or null otherwise.</returns>
        Student? FindWithEmail(string email);

        /// <summary>
        /// Gets the students, which are still studying.
        /// </summary>
        /// <returns></returns>
        List<Student> GetCurrentStudents();

        void AddModuleToUser(DiscordAccount discordAccount, Module module);
    }
}
