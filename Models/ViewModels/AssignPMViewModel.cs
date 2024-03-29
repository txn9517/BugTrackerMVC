﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTrackerMVC.Models.ViewModels
{
    public class AssignPMViewModel
    {
        public Project? Project { get; set; }

        public SelectList? PMList { get; set; }

        public string? PMId { get; set; }

        public MultiSelectList? DevList { get; set; }

        public List<string> SelectedDevelopers { get; set; } = new();

        public List<string> SelectedSubmitters { get; set; } = new();

        public string? DevId { get; set; }

        public MultiSelectList? SubList { get; set; }

        public string? SubId { get; set; }
    }
}
