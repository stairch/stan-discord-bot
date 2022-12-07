using LinqToDB;
using StanDatabase.DTOs;
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
                            .Set(s => s.FkHouseId, student.FkHouseId)
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
                return db.Student.LoadWith(h => h.House).SingleOrDefault(s => s.StudentEmail == email);
            }
        }

        public List<StudentsPerHouseDTO> NumberOfStudentsPerHouse()
        {
            using (var db = new DbStan())
            {
                var query = from s in db.Student
                            join h in db.House on s.FkHouseId equals h.HouseId
                            group s by h.Name into g
                            select new StudentsPerHouseDTO
                            {
                                HouseName = g.Key,
                                StudentsCount = g.Count()
                            };
                return query.ToList();
            }
        }

        public List<StudentsPerSemesterDTO> NumberOfStudentsPerSemester()
        {
            using (var db = new DbStan())
            {
                var query = from s in db.Student
                            group s by s.Semester into g
                            select new StudentsPerSemesterDTO
                            {
                                Semester = g.Key,
                                StudentsCount = g.Count()
                            };
                return query.ToList();
            }
        }

        public List<Student> GetCurrentStudents()
        {
            using (var db = new DbStan())
            {
                return db.Student.Where(s => s.StillStudying).ToList();
            }
        }

        public void AddModuleToUser(DiscordAccount discordAccount, Module module)
        {
            using (var db = new DbStan())
            {
                db.Insert(
                    new DiscordAccountModule(
                        DateTime.Now,
                        discordAccount.DiscordAccountId,
                        discordAccount,
                        module.ModuleId,
                        module
                    )
                );
            }
        }
    }
}
