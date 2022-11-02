using StanDatabase;
using StanDatabase.Models;
using StanDatabase.Repositories;
using StanScript;

namespace StanScripts
{
    public class LoadStudents
    {
        private readonly IStudentRepository _studentRepository;

        public LoadStudents(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public void LoadStudentsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine("Error: File not found! Check your path.");
                return;
            }

            StreamReader reader = new StreamReader(File.OpenRead(filePath));

            IList<string> columnNames = CsvHelper.GetCsvValuesOnNextLine(reader).ToList();
            Console.WriteLine($"Columns in file: {String.Join(", ", columnNames)}");

            int emailIndex = columnNames.IndexOf(StanSettings.EmailColumnNameInCsv);
            int houseIndex = columnNames.IndexOf(StanSettings.HouseColumnNameInCsv);
            int semesterIndex = columnNames.IndexOf(StanSettings.SemesterColumnNameInCsv);

            Console.WriteLine($"{nameof(emailIndex)}: {emailIndex} | {nameof(houseIndex)}: {houseIndex} | {nameof(semesterIndex)}: {semesterIndex}");

            IList<Student> currentStudents = new List<Student>();
            while (!reader.EndOfStream)
            {
                string[] values = CsvHelper.GetCsvValuesOnNextLine(reader);

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

            //_studentRepository.InsertMultiple(currentStudents);

            if (ShouldOldStudentsBeMarkedAsExstudents())
            {
                //_studentRepository.DeactivateOldStudents(currentStudents);
            }
        }

        private bool ShouldOldStudentsBeMarkedAsExstudents()
        {
            string question = $"Are students that are not in this list exstudents? ({ConsoleHelper.YesAnswer}/{ConsoleHelper.NoAnswer})" +
                $"Answering with {ConsoleHelper.YesAnswer} sets {nameof(Student.StillStudying)} to false on the other students";
            return ConsoleHelper.YesNoQuestion(question);
        }
    }
}
