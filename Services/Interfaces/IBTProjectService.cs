﻿using BugTrackerMVC.Models;

namespace BugTrackerMVC.Services.Interfaces
{
    // actions for Project
    public interface IBTProjectService
    {
        // get all projects for specific company
        public Task<List<Project>> GetAllProjectsByCompanyIdAsync(int companyId);

        // get archived projects for specific company
        public Task<List<Project>> GetArchivedProjectByCompanyIdAsync(int companyId);

        // get user projects
        public Task<List<Project>?> GetUserProjectsAsync(string userId);

        // add project to db
        public Task AddProjectAsync(Project project);

        // get project by id
        public Task<Project> GetProjectByIdAsync(int projectId, int companyId);

        // update project to db
        public Task UpdateProjectAsync(Project project);

        // get project priority list
        public Task<IEnumerable<ProjectPriority>> GetProjectPrioritiesAsync();

        // get project manager
        public Task<BTUser> GetProjectManagerAsync(int projectId);

        // add project manager
        public Task<bool> AddProjectManagerAsync(string userId, int projectId);

        // remove project manager
        public Task RemoveProjectManagerAsync(int projectId);

        // add Member to project
        public Task<bool> AddMemberToProjectAsync(BTUser member, int projectId);

        // remove Member from project
        public Task<bool> RemoveMemberFromProjectAsync(BTUser member, int projectId);
    }
}
