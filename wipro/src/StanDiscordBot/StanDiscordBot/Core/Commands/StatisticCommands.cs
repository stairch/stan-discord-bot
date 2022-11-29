﻿using Discord;
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

        StatisticCommands(IStudentRepository studentRepository) 
        {
            _studentRepository = studentRepository;

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
            plt.SetAxisLimits(yMin: 0);

            await Context.Channel.SendFileAsync(plt.SaveFig("img/studentsPerHouse.png"));
        }

        [Command("studentsPerSemester", true)]
        [RequireRole("stair")]
        [RequireRole("administrator")]
        [Discord.Commands.Summary("Plots number of students per house.")]
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
            plt.SetAxisLimits(yMin: 0);

            await Context.Channel.SendFileAsync(plt.SaveFig("img/studentsPerSemester.png"));
        }
    }
}
