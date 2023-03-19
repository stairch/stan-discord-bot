using Discord.Commands;
using Discord.Interactions;
using ScottPlot.Plottable;
using StanDatabase.Repositories;
using StanDatabase.DTOs;
using NLog;
using StanBot.Services.ErrorNotificactionService;
using System;
using System.Drawing;

namespace StanBot.Core.Commands
{
    public class StatisticCommands : ModuleBase<SocketCommandContext>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly DatabaseErrorNotificationService _databaseErrorNotificationService;

        private readonly MailErrorNotificationService _mailErrorNotificationService;

        private readonly IStudentRepository _studentRepository;
        private readonly IDiscordAccountRepository _discordAccountRepository;
        private readonly IDiscordAccountModuleRepository _discordAccountModuleRepository;

        StatisticCommands(
            IStudentRepository studentRepository,
            IDiscordAccountRepository discordAccountRepository,
            IDiscordAccountModuleRepository discordAccountModuleRepository,
            DatabaseErrorNotificationService databaseErrorNotificationService,
            MailErrorNotificationService mailErrorNotificationService)
        {
            _studentRepository = studentRepository;
            _discordAccountRepository = discordAccountRepository;
            _discordAccountModuleRepository = discordAccountModuleRepository;
            _databaseErrorNotificationService = databaseErrorNotificationService;
            _mailErrorNotificationService = mailErrorNotificationService;

            // Creates image directory for plots, if it doesnt exist yet
            if (!Directory.Exists("img"))
            {
                _logger.Debug("Create img folder for statistic diagrams");
                Directory.CreateDirectory("img");
            }
        }

        [Command("studentsPerHouse", true)]
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Plots number of students per house.")]
        public async Task StudentsPerHouse()
        {
            _logger.Info($"Received statistic command !studentsPerHouse from {Context.Message.Author.Username}.");

            try
            {
                if (Context.Channel.Name.Equals("bot-commands"))
                {
                    List<StudentsPerHouseDTO> list;
                    try
                    {
                        list = _studentRepository.NumberOfStudentsPerHouse();
                    }
                    catch(Exception ex)
                    {
                        _databaseErrorNotificationService.SendDatabaseErrorToAdmins(ex, "StatisticCommand.studentsPerHouse");
                        _logger.Error($"{ex.GetType()} There was an Error, due to a database exception. Admin has been contacted. Stacktrace: {ex.Message}");
                        await Context.Channel.SendMessageAsync("Es gab einen Fehler während dem Ausführen des Commands. Ein Administrator wurde schon kontaktiert. " +
                            "Bitte habe etwas Geduld und versuche es später erneut.\n\r" +
                            "There was an error while executing the command. An administrator has already been contacted. Please be patient and try again later.");
                        return;
                    }

                    //createStudentPerHousePlot(list);
                    createStudentPerHousePlotScottplot(list);
                    
                    await Context.Channel.SendFileAsync("img/studentsPerHouse.png");
                }
            }
            catch(Exception ex)
            {
                // TODO: use different error notification service
                _mailErrorNotificationService.SendMailErrorToAdmins(ex, "StatisticCommand.studentsPerHouse");
                _logger.Error($"{ex.GetType()} There was an Error, while creating the diagram. Admin has been contacted. Message: {ex.Message} | Stacktrace: {ex.StackTrace}");
                await Context.Channel.SendMessageAsync("Es gab einen Fehler während dem Ausführen des Commands. Ein Administrator wurde schon kontaktiert. " +
                    "Bitte habe etwas Geduld und versuche es später erneut.\n\r" +
                    "There was an error while executing the command. An administrator has already been contacted. Please be patient and try again later.");
            }
        }

        [Command("studentsPerSemester", true)]
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Plots number of students per semester.")]
        public async Task StudentsPerSemester()
        {
            _logger.Info($"Received statistic command !studentsPerSemester from {Context.Message.Author.Username}.");
            //await ReplyAsync($"Not implemented yet.");
            try
            {
                if (Context.Channel.Name.Equals("bot-commands"))
                {
                    List<StudentsPerSemesterDTO> list = _studentRepository.NumberOfStudentsPerSemester();

                    List<Bar> bars = new();
                    string[] labels = new string[list.Count];

                    for (int i = 0; i < list.Count; i++)
                    {
                        Bar bar = new()
                        {
                            Value = list[i].StudentsCount,
                            Position = i,
                            Label = list[i].StudentsCount.ToString(),
                            FillColor = ScottPlot.Palette.Category10.GetColor(i),
                            LineWidth = 2,
                        };
                        bars.Add(bar);

                        labels[i] = list[i].Semester.ToString();
                    }

                    var plt = new ScottPlot.Plot(600, 400);
                    plt.AddBarSeries(bars);
                    plt.XTicks(labels);
                    plt.Title("Students per Semester");
                    plt.XLabel(nameof(StudentsPerSemesterDTO.Semester));
                    plt.YLabel(nameof(StudentsPerSemesterDTO.StudentsCount));
                    plt.SetAxisLimits(yMin: 0);

                    await Context.Channel.SendFileAsync(plt.SaveFig("img/studentsPerSemester.png"));
                }
            }
            catch(Exception ex)
            {
                _databaseErrorNotificationService.SendDatabaseErrorToAdmins(ex, "StatisticCommand.studentsPerSemester");
                _logger.Error($"There was an Error, due to a database exception. Admin has been contacted. Stacktrace: {ex.Message}");
                await Context.Channel.SendMessageAsync("Es gab einen Fehler während dem Ausführen des Commands. Ein Administrator wurde schon kontaktiert. " +
                    "Bitte habe etwas Geduld und versuche es später erneut.\n\r" +
                    "There was an error while executing the command. An administrator has already been contacted. Please be patient and try again later.");
            }
        }

        [Command("accountsPerSemester", true)]
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Plots number of discord accounts registered, per semester.")]
        public async Task AccountsPerSemester()
        {
            _logger.Info($"Received statistic command !accountsPerSemester from {Context.Message.Author.Username}.");
            //await ReplyAsync($"Not implemented yet");
            try
            {
                if (Context.Channel.Name.Equals("bot-commands"))
                {
                    List<DiscordAccountsPerSemesterDTO> list = _discordAccountRepository.NumberOfDiscordAccountsPerSemester();

                    List<Bar> bars = new();
                    string[] labels = new string[list.Count];

                    for (int i = 0; i < list.Count; i++)
                    {
                        Bar bar = new()
                        {
                            Value = list[i].AccountsCount,
                            Position = i,
                            Label = list[i].AccountsCount.ToString(),
                            FillColor = ScottPlot.Palette.Category10.GetColor(i),
                            LineWidth = 2,
                        };
                        bars.Add(bar);

                        labels[i] = list[i].Semester.ToString();
                    }

                    var plt = new ScottPlot.Plot(600, 400);
                    plt.AddBarSeries(bars);
                    plt.XTicks(labels);
                    plt.Title("Discord Accounts per Semester");
                    plt.XLabel(nameof(DiscordAccountsPerSemesterDTO.Semester));
                    plt.YLabel(nameof(DiscordAccountsPerSemesterDTO.AccountsCount));
                    plt.SetAxisLimits(yMin: 0);

                    await Context.Channel.SendFileAsync(plt.SaveFig("img/accountsPerSemester.png"));
                }
            }
            catch(Exception ex)
            {
                _databaseErrorNotificationService.SendDatabaseErrorToAdmins(ex, "StatisticCommand.accountsPerSemester");
                _logger.Error($"There was an Error, due to a database exception. Admin has been contacted. Stacktrace: {ex.Message}");
                await Context.Channel.SendMessageAsync("Es gab einen Fehler während dem Ausführen des Commands. Ein Administrator wurde schon kontaktiert. " +
                    "Bitte habe etwas Geduld und versuche es später erneut.\n\r" +
                    "There was an error while executing the command. An administrator has already been contacted. Please be patient and try again later.");
            }
        }

        [Command("membersPerModule", true)]
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Plots number of members for the top n modules.")]
        public async Task MembersPerModule(int numberOfModules = 10)
        {
            _logger.Info($"Received statistic command !memebersPerModule from {Context.Message.Author.Username}.");
            //await ReplyAsync($"Not implemented yet");
            try
            {
                List<MembersPerModuleDTO> list = _discordAccountModuleRepository.NumberOfMembersPerModule(numberOfModules);

                string[] moduleName = new string[list.Count];
                double[] memberCount = new double[list.Count];

                for (int i = 0; i < list.Count; i++)
                {
                    moduleName[i] = list[i].ModuleName;
                    memberCount[i] = list[i].MemberCount;
                }

                var plt = new ScottPlot.Plot(600, 400);
                var pie = plt.AddPie(memberCount);
                pie.SliceLabels = moduleName;
                pie.ShowValues = true;
                plt.Legend();
                plt.Title($"Members per Module Top {numberOfModules}");

                await Context.Channel.SendFileAsync(plt.SaveFig("img/membersPerModule.png"));
            }
            catch(Exception ex )
            {
                _databaseErrorNotificationService.SendDatabaseErrorToAdmins(ex, "StatisticCommand.membersPerModule");
                _logger.Error($"There was an Error, due to a database exception. Admin has been contacted. Stacktrace: {ex.Message}");
                await Context.Channel.SendMessageAsync("Es gab einen Fehler während dem Ausführen des Commands. Ein Administrator wurde schon kontaktiert. " +
                    "Bitte habe etwas Geduld und versuche es später erneut.\n\r" +
                    "There was an error while executing the command. An administrator has already been contacted. Please be patient and try again later.");
            }
        }

        private void createStudentPerHousePlotScottplot(List<StudentsPerHouseDTO> list)
        {
            string[] labels = new string[list.Count()];
            List<Bar> bars = new();

            for (int i = 0; i < list.Count; i++)
            {
                Bar bar = new()
                {
                    Value = list[i].StudentsCount,
                    Position = i,
                    FillColor = System.Drawing.Color.FromName(list[i].HouseName),
                    Label = list[i].StudentsCount.ToString(),
                    LineWidth = 2,
                };
                bars.Add(bar);

                labels[i] = list[i].HouseName;
            }

            var plt = new ScottPlot.Plot(600, 400);
            plt.AddBarSeries(bars);
            plt.XTicks(labels);
            plt.Title("Students per House");
            plt.XLabel(nameof(StudentsPerHouseDTO.HouseName));
            plt.YLabel(nameof(StudentsPerHouseDTO.StudentsCount));
            plt.SetAxisLimits(yMin: 0);
            plt.SaveFig("img/studentsPerHouse.png");

            //await Context.Channel.SendFileAsync();
        }

        /*private void createStudentPerHousePlot(List<StudentsPerHouseDTO> students)
        {
            var model = new PlotModel
            {
                Title = "Students per House"
            };

            var barSeries = new OxyPlot.Series.BarSeries();
            var catergoryAxis = new CategoryAxis();

            for (int i = 0; i < students.Count; i++)
            {
                BarItem bar = new()
                {
                    Value = students[i].StudentsCount,
                };
                catergoryAxis.Labels.Add(students[i].HouseName);
                barSeries.Items.Add(bar);
            }

            model.Series.Add(barSeries);
            model.Axes.Add( catergoryAxis);

            using (var stream = new MemoryStream())
            {
                var pngExporter = new PngExporter { Width = 600, Height = 400, Background = OxyColors.White };
                pngExporter.Export(model, stream);
                using (var file = new FileStream("img/studentsPerHouse.png", FileMode.Create, FileAccess.Write))
                {
                    stream.WriteTo(file);
                }
            }
        }*/
    }
}
