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
                IEnumerable<string> newStudentMails = students
                    .Select(s => s.StudentEmail);

                IEnumerable<string> currentStudentMails = db.Student
                    .Select(s => s.StudentEmail);

                IList<string> oldStudentMails = currentStudentMails.Except(newStudentMails).ToList();

                foreach (string mail in oldStudentMails)
                {
                    db.Student
                            .Where(s => s.StudentEmail == mail)
                            .Set(s => s.StillStudying, false)
                            .Update();
                    // Discord Roles on the Server will be updated through the UpdateStudentsCommand
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

        public void SetStudentIsAdmin(Student student, bool isAdmin)
        {
            using (var db = new DbStan())
            {
                db.Student
                    .Where(s => s.StudentEmail == student.StudentEmail)
                    .Set(s => s.IsDiscordAdmin, isAdmin)
                    .Update();
            }
        }

        public IList<Student> GetAllDiscordAdmins()
        {
            using (var db = new DbStan())
            {
                return db.Student
                    .Where(s => s.IsDiscordAdmin == true)
                    .ToList();
            }
        }
        
        /// <summary>
        /// Add module to user when connection doesn't exist yet
        /// </summary>
        /// <param name="discordAccount"></param>
        /// <param name="module"></param>
        public void AddModuleToUser(DiscordAccount discordAccount, Module module)
        {
            using (var db = new DbStan())
            {
                if (!db.DiscordAccountModule.Any(dam => dam.DiscordAccount.Equals(discordAccount) && dam.Module.Equals(module)))
                {
                    db.Insert(
                        DiscordAccountModule.CreateNew(
                            discordAccount,
                            module
                        )
                    );
                }
            }
        }

        /// <summary>
        /// Add module to user when connection doesn't exist yet
        /// </summary>
        /// <param name="discordAccount"></param>
        /// <param name="module"></param>
        public void RemoveUserFromModule(DiscordAccount discordAccount, Module module)
        {
            using (var db = new DbStan())
            {
                db.DiscordAccountModule.Delete(dam => dam.DiscordAccount.Equals(discordAccount) && dam.Module.Equals(module));
                //if ()
                //{
                //    db.Delete(
                //        new DiscordAccountModule(
                //            DateTime.Now,
                //            discordAccount.DiscordAccountId,
                //            discordAccount,
                //            module.ModuleId,
                //            module
                //        )
                //    );
                //}
            }
        }
    }
}
