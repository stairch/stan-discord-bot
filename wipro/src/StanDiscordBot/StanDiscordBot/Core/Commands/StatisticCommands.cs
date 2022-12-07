using Discord.Commands;
using Discord.Interactions;
using ScottPlot.Plottable;
using StanDatabase.Repositories;
using StanDatabase.DTOs;

namespace StanBot.Core.Commands
{
    public class StatisticCommands : ModuleBase<SocketCommandContext>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IDiscordAccountRepository _discordAccountRepository;
        private readonly IDiscordAccountModuleRepository _discordAccountModuleRepository;

        StatisticCommands(
            IStudentRepository studentRepository,
            IDiscordAccountRepository discordAccountRepository,
            IDiscordAccountModuleRepository discordAccountModuleRepository) 
        {
            _studentRepository = studentRepository;
            _discordAccountRepository = discordAccountRepository;
            _discordAccountModuleRepository = discordAccountModuleRepository;

            // Creates image directory for plots, if it doesnt exist yet
            if (!Directory.Exists("img"))
                Directory.CreateDirectory("img");
        }

        [Command("studentsPerHouse", true)]
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Plots number of students per house.")]
        public async Task StudentsPerHouse()
        {

            List<StudentsPerHouseDTO> list = _studentRepository.NumberOfStudentsPerHouse();

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

            await Context.Channel.SendFileAsync(plt.SaveFig("img/studentsPerHouse.png"));
        }

        [Command("studentsPerSemester", true)]
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Plots number of students per semester.")]
        public async Task StudentsPerSemester()
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

        [Command("accountsPerSemester", true)]
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Plots number of discord accounts registered, per semester.")]
        public async Task AccountsPerSemester()
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

        [Command("membersPerModule", true)]
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Plots number of members for the top n modules.")]
        public async Task MembersPerModule(int numberOfModules = 10)
        {
            List<MembersPerModuleDTO> list = _discordAccountModuleRepository.NumberOfMembersPerModule(numberOfModules);

            string[] moduleName = new string[list.Count];
            double[] memberCount = new double[list.Count];

            for(int i = 0; i < list.Count; i++)
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
    }
}
