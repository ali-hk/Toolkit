using System;
using System.Collections.Generic;

namespace Toolkit.TestApp.Models
{
    public class Goaltender
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Number { get; set; }

        public DateTime Born { get; set; }

        public string NationalTeam { get; set; }

        public string Photo { get; set; }

        public int GamesPlayed { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int Ties { get; set; }

        public int Minutes { get; set; }

        public int GoalsAgainst { get; set; }

        public int Shootouts { get; set; }

        public float GoalsAgainstAverage { get; set; }

        public float SavePercentage { get; set; }

        public IReadOnlyCollection<Team> Teams { get; set; }
    }
}