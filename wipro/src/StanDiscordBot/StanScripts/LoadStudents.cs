using NLog;
using StanDatabase;
using StanDatabase.DataAccessLayer;
using StanDatabase.Models;
using StanDatabase.Repositories;
using StanDatabase.Util;
using StanScript;

namespace StanScripts
{
    public class LoadStudents
    {
        public const string COMMAND_NAME = "loadStudents";

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IStudentRepository _studentRepository;

        private readonly IHouseRepository _houseRepository;

        public LoadStudents(IStudentRepository studentRepository, IHouseRepository houseRepository)
        {
            _studentRepository = studentRepository;
            _houseRepository = houseRepository;
        }

        public void LoadStudentsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                string errorMessage = "Error: File not found! Check your path.";
                _logger.Error(errorMessage);
                Console.Error.WriteLine(errorMessage);
                return;
            }

            StreamReader reader = new StreamReader(File.OpenRead(filePath));

            IList<string> columnNames = CsvHelper.GetCsvValuesOnNextLine(reader).ToList();
            string columnsLogMessage = $"Columns in file: {String.Join(", ", columnNames)}";
            _logger.Info(columnsLogMessage);
            Console.WriteLine(columnsLogMessage);

            int emailIndex = columnNames.IndexOf(StanDatabaseConfigLoader.Get().EmailColumnNameInCsv);
            int houseIndex = columnNames.IndexOf(StanDatabaseConfigLoader.Get().HouseColumnNameInCsv);
            int semesterIndex = columnNames.IndexOf(StanDatabaseConfigLoader.Get().SemesterColumnNameInCsv);

            string logMessage = $"{nameof(emailIndex)}: {emailIndex} | {nameof(houseIndex)}: {houseIndex} | {nameof(semesterIndex)}: {semesterIndex}";
            Console.WriteLine(logMessage);
            _logger.Info(logMessage);

            IList<Student> currentStudents = new List<Student>();
            while (!reader.EndOfStream)
            {
                string[] values = CsvHelper.GetCsvValuesOnNextLine(reader);

                string email = values[emailIndex].Trim();
                if (!StudentUtil.IsStudentEmailFormatValid(email))
                {
                    string errorMessage = $"Student email format is wrong! No changes made! Fix it and retry the whole file. Email: {email}";
                    Console.Error.WriteLine(errorMessage);
                    _logger.Error(errorMessage);
                    return;
                }

                string houseName = values[houseIndex];
                if (!_houseRepository.IsHouseNameValid(houseName))
                {
                    string errorMessage = $"House name doesn't exist! No changes made! Fix it and retry the whole file. House name: {houseName}";
                    Console.Error.WriteLine(errorMessage);
                    _logger.Error(errorMessage);
                    return;
                }

                if (!int.TryParse(values[semesterIndex], out int semester))
                {
                    string errorMessage = $"Semester is not a number! No changes made! Fix it and retry the whole file. Semester: {values[semesterIndex]}";
                    Console.Error.WriteLine(errorMessage);
                    _logger.Error(errorMessage);
                    return;
                }

                //Student currentStudent = new Student(
                //    email,
                //    House.GetHouseIdByName(values[houseIndex]),
                //    true,
                //    semester
                //);
                Student currentStudent = new Student(
                    email,
                    _houseRepository.GetHouseByName(values[houseIndex]),
                    true,
                    semester
                );
                currentStudents.Add(currentStudent);
            }

            _logger.Info("Loaded all students (not added to DB yet).");
            _studentRepository.InsertMultiple(currentStudents);

            if (ShouldOldStudentsBeMarkedAsExstudents())
            {
                _studentRepository.DeactivateOldStudents(currentStudents);
            }
        }

        private bool ShouldOldStudentsBeMarkedAsExstudents()
        {
            string question = $"Are students that are not in this list exstudents? ({ConsoleHelper.YesAnswer}/{ConsoleHelper.NoAnswer})" +
                $"\nAnswering with {ConsoleHelper.YesAnswer} sets {nameof(Student.StillStudying)} to false on the other students";
            return ConsoleHelper.YesNoQuestion(question);
        }
    }
}
