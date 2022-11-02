using LinqToDB;
using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanDatabase.DataAccessLayer
{
    public class StudentRepository : IStudentRepository
    {
        public void InsertMultiple(IList<Student> students)
        {
            using (var db = new DbStan())
            {
                foreach (Student student in students)
                {
                    if (!db.Student.Any(s => s.StudentEmail == student.StudentEmail))
                    {
                        db.Insert(student);
                    }
                    else
                    {
                        db.Student
                            .Where(s => s.StudentEmail == student.StudentEmail)
                            .Set(s => s.Semester, student.Semester)
                            .Set(s => s.StillStudying, student.StillStudying)
                            .Set(s => s.House, student.House)
                            .Update();
                    }
                }
            }
        }

        public void DeactivateOldStudents(IList<Student> students)
        {
            using (var db = new DbStan())
            {
                IList<string> currentStudentEmails = students
                    .Select(cs => cs.StudentEmail)
                    .ToList();

                IList<Student> oldStudents = db.Student
                    .Where(s => currentStudentEmails.Contains(s.StudentEmail))
                    .ToList();

                foreach (Student oldStudent in oldStudents)
                {
                    db.Student
                            .Where(s => s.StudentEmail == oldStudent.StudentEmail)
                            .Set(s => s.StillStudying, false)
                            .Update();
                    // TODO: how to update discord role on server from here?
                }
            }
        }

        public Student? FindWithEmail(string email)
        {
            using (var db = new DbStan())
            {
                var student = from s in db.Student
                              where s.StudentEmail == email
                              select s;

                if (student.Count() == 0) return null;

                return student.First();
            }
        }
    }
}
