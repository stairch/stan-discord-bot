using LinqToDB;
using StanDatabase;

namespace StanScripts
{
    internal static class LoadStudents
    {
        public static void LoadStudentsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine("Error: File not found! Check your path.");
                return;
            }

            StreamReader reader = new StreamReader(File.OpenRead(filePath));

            IList<string> columnNames = getCsvValuesOnNextLine(reader).ToList();
            Console.WriteLine($"Columns in file: {String.Join(", ", columnNames)}");

            int emailIndex = columnNames.IndexOf(StanSettings.emailColumnNameInCsv);
            int houseIndex = columnNames.IndexOf(StanSettings.houseColumnNameInCsv);
            int semesterIndex = columnNames.IndexOf(StanSettings.semesterColumnNameInCsv);

            Console.WriteLine($"{nameof(emailIndex)}: {emailIndex} | {nameof(houseIndex)}: {houseIndex} | {nameof(semesterIndex)}: {semesterIndex}");

            IList<Student> currentStudents = new List<Student>();
            while (!reader.EndOfStream)
            {
                string[] values = getCsvValuesOnNextLine(reader);

                string email = values[emailIndex].Trim();
                if (!Student.IsStudentEmailFormatValid(email))
                {
                    Console.Error.WriteLine($"Student email format is wrong! No changes made! Fix it and retry the whole file. Email: {email}");
                    return;
                }

                string houseName = values[houseIndex];
                if (!House.IsHouseNameValid(houseName))
                {
                    Console.Error.WriteLine($"House name doesn't exist! No changes made! Fix it and retry the whole file. House name: {houseName}");
                    return;
                }

                if (!int.TryParse(values[semesterIndex], out int semester))
                {
                    Console.Error.WriteLine($"Semester is not a number! No changes made! Fix it and retry the whole file. Semester: {values[semesterIndex]}");
                    return;
                }

                Student currentStudent = new Student(
                    email,
                    House.GetHouseIdByName(values[houseIndex]),
                    true,
                    semester
                );
                currentStudents.Add(currentStudent);
                Console.WriteLine(currentStudent);
            }

            //InsertStudentsIntoDb(currentStudents);

            if (ShouldOldStudentsBeMarkedAsExstudents())
            {
                //DeactivateOldStudents(currentStudents);
            }
        }

        private static void InsertStudentsIntoDb(IList<Student> currentStudents)
        {
            using (var db = new DbStan())
            {
                foreach (Student student in currentStudents)
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

        private static bool ShouldOldStudentsBeMarkedAsExstudents()
        {
            string yesAnswer = "y";
            string noAnswer = "n";

            string answer;
            do
            {
                Console.WriteLine($"Are students that are not in this list exstudents? ({yesAnswer}/{noAnswer})");
                Console.WriteLine($"Answering with {yesAnswer} sets {nameof(Student.StillStudying)} to false on the other students");
                answer = Console.ReadLine().ToLower();
                if (answer != yesAnswer && answer != noAnswer)
                {
                    Console.Error.WriteLine("Unknown input! Try again.");
                }
            } while (answer != yesAnswer && answer != noAnswer);

            return answer == yesAnswer;
        }

        /// <summary>
        /// deactivates all students that aren't in the given list.
        /// </summary>
        /// <param name="currentStudents"></param>
        private static void DeactivateOldStudents(IList<Student> currentStudents)
        {
            using (var db = new DbStan())
            {
                IList<string> currentStudentEmails = currentStudents
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

        private static string[] getCsvValuesOnNextLine(StreamReader reader)
        {
            string line = reader.ReadLine();
            line = line.Trim();
            string[] values = line.Split(StanSettings.Separator);
            return values;
        }
    }
}
