﻿using BugTrackerMVC.Data;
using BugTrackerMVC.Models;
using BugTrackerMVC.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugTrackerMVC.Models.Enums;
using System.ComponentModel.Design;

namespace BugTrackerMVC.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _btRolesService;
        private readonly UserManager<BTUser> _userManager;
        
        public BTProjectService(ApplicationDbContext context,
                                IBTRolesService btRolesService,
                                UserManager<BTUser> userManager)
        {
            _context = context;
            _btRolesService = btRolesService;
            _userManager = userManager;
        }

        public async Task<List<Project>> GetAllProjectsByCompanyIdAsync(int companyId)
        {
            try
            {
                List<Project> projects = await _context.Projects
                                                       .Where(p => p.CompanyId == companyId)
                                                       .Include(p => p.Company)
                                                       .Include(p => p.ProjectPriority)
                                                       .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.TicketPriority)
                                                       .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.TicketType)
                                                       .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.TicketStatus)
                                                       .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.Comments)
                                                       .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.Attachments)
                                                       .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.History)
                                                       .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.DeveloperUser)
                                                       .Include(p => p.Tickets)
                                                            .ThenInclude(t => t.SubmitterUser)
                                                       .Include(p => p.Members)
                                                       .OrderByDescending(p => p.Created)
                                                            .ThenByDescending(p => p.StartDate)
                                                            .ThenByDescending(p => p.EndDate)
                                                       .ToListAsync();

                return projects;

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetArchivedProjectByCompanyIdAsync(int companyId)
        {
            try
            {
                List<Project> projects = await _context.Projects
                                                       .Where(p => p.CompanyId == companyId && p.Archived == true)
                                                       .Include(p => p.Company)
                                                       .Include(p => p.ProjectPriority)
                                                       .Include(p => p.Tickets)
                                                       .Include(p => p.Members)
                                                       .OrderByDescending(p => p.Created)
                                                            .ThenByDescending(p => p.StartDate)
                                                            .ThenByDescending(p => p.EndDate)
                                                       .ToListAsync();

                return projects;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>?> GetUserProjectsAsync(string userId)
        {
            try
            {
                // get company id of user
                int companyId = (await _userManager.FindByIdAsync(userId)).CompanyId;

                // check the role the user is in
                if (await _btRolesService.IsUserInRoleAsync(await _userManager.FindByIdAsync(userId), nameof(BTRoles.Admin)))
                {
                    // if the user is an admin, get all projects
                    List<Project>? projects = (await GetAllProjectsByCompanyIdAsync(companyId)).Where(p => p.Archived == false).ToList();

                    return projects;
                }
                // otherwise, get the projects the user is a member of
                else
                {
                    List<Project>? projects = (await _context.Users
                                                            .Include(u => u.Projects)
                                                                .ThenInclude(p => p.ProjectPriority)
                                                            .Include(u => u.Projects)
                                                                .ThenInclude(p => p.Members)
                                                            .Include(u => u.Projects)
                                                                .ThenInclude(p => p.Tickets)
                                                                    .ThenInclude(t => t.TicketPriority)
                                                            .Include(u => u.Projects)
                                                                .ThenInclude(p => p.Tickets)
                                                                    .ThenInclude(t => t.TicketType)
                                                            .Include(u => u.Projects)
                                                                .ThenInclude(p => p.Tickets)
                                                                    .ThenInclude(t => t.TicketStatus)
                                                            .Include(u => u.Projects)
                                                                .ThenInclude(p => p.Tickets)
                                                                    .ThenInclude(t => t.Comments)
                                                            .Include(u => u.Projects)
                                                                .ThenInclude(p => p.Tickets)
                                                                    .ThenInclude(t => t.Attachments)
                                                            .Include(u => u.Projects)
                                                                .ThenInclude(p => p.Tickets)
                                                                    .ThenInclude(t => t.History)
                                                            .Include(u => u.Projects)
                                                                .ThenInclude(p => p.Tickets)
                                                                    .ThenInclude(t => t.DeveloperUser)
                                                            .Include(u => u.Projects)
                                                                .ThenInclude(p => p.Tickets)
                                                                    .ThenInclude(t => t.SubmitterUser)
                                                            .FirstOrDefaultAsync(u => u.Id == userId))?
                                                            .Projects.Where(p => p.Archived == false)
                                                            .ToList();

                    return projects;
                }

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task AddProjectAsync(Project project)
        {
            try
            {
                _context.Add(project);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            try
            {
                Project? project = await _context.Projects
                                                 .Include(p => p.Company)
                                                 .Include(p => p.Members)
                                                 .Include(p => p.ProjectPriority)
                                                 .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketPriority)
                                                 .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketType)
                                                 .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketStatus)
                                                 .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.Comments)
                                                 .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.Attachments)
                                                 .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.History)
                                                 .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.DeveloperUser)
                                                 .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.SubmitterUser)
                                                 .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

                return project!;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            try
            {
                _context.Update(project);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                // set Archived property to true
                project.Archived = true;

                await UpdateProjectAsync(project);

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task RestoreProjectAsync(Project project)
        {
            try
            {
                // set Archived property to false
                project.Archived = false;

                await UpdateProjectAsync(project);
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            try
            {
                IEnumerable<ProjectPriority> projectPriorities = await _context.ProjectPriorities
                                                                        .ToListAsync();

                return projectPriorities;

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            try
            {
                Project? project = await _context.Projects
                                                .Include(p => p.Members)
                                                .FirstOrDefaultAsync(p => p.Id == projectId);

                foreach(BTUser member in project!.Members)
                {
                    if (await _btRolesService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                    {
                        return member;
                    }
                }

                return null!;

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task<BTUser> GetDeveloperAsync(int projectId)
        {
            try
            {
                Project? project = await _context.Projects
                                                .Include(p => p.Members)
                                                .FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BTUser member in project!.Members)
                {
                    if (await _btRolesService.IsUserInRoleAsync(member, nameof(BTRoles.Developer)))
                    {
                        return member;
                    }
                }

                return null!;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BTUser> GetSubmitterAsync(int projectId)
        {
            try
            {
                Project? project = await _context.Projects
                                                .Include(p => p.Members)
                                                .FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BTUser member in project!.Members)
                {
                    if (await _btRolesService.IsUserInRoleAsync(member, nameof(BTRoles.Submitter)))
                    {
                        return member;
                    }
                }

                return null!;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            try
            {
                BTUser? currentPM = await GetProjectManagerAsync(projectId);
                BTUser? selectedPM = await _context.Users.FindAsync(userId);

                // remove current PM
                if (currentPM != null)
                {
                    await RemoveProjectManagerAsync(projectId);
                }

                // add new PM
                // try to add a member to project
                try
                {
                    await AddMemberToProjectAsync(selectedPM!, projectId);

                    await _context.SaveChangesAsync();

                    return true;

                } catch (Exception)
                {
                    throw;
                }

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            try
            {
                BTUser? currentPM = await GetProjectManagerAsync(projectId);

                // remove current PM
                if (currentPM != null)
                {
                    await RemoveMemberFromProjectAsync(currentPM, projectId);

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddMemberToProjectAsync(BTUser member, int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId, member.CompanyId);

                bool IsOnProject = project.Members.Any(m => m.Id == member.Id);

                if (!IsOnProject)
                {
                    project.Members.Add(member);
                    
                    await _context.SaveChangesAsync();

                    return true;
                }

                return false;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> RemoveMemberFromProjectAsync(BTUser member, int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId, member.CompanyId);

                bool IsOnProject = project.Members.Any(m => m.Id == member.Id);

                if (IsOnProject)
                {
                    project.Members.Remove(member);

                    await _context.SaveChangesAsync();

                    return true;
                }

                return false;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priority)
        {
            try
            {
                List<Project> projects = _context.Projects
                                                        .Where(p => p.CompanyId == companyId)
                                                        .Where(p => p.ProjectPriority!.Name!.Equals(priority))
                                                        .ToList();

                return projects;

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string roleName)
        {
            try
            {
                // get project
                Project? project = await _context.Projects
                                                        .Include(p => p.Members)
                                                        .FirstOrDefaultAsync(p => p.Id == projectId);

                List<BTUser> developers = new List<BTUser>();

                foreach (BTUser member in project!.Members)
                {
                    if (await _btRolesService.IsUserInRoleAsync(member, nameof(BTRoles.Developer)))
                    {
                        developers.Add(member);
                    }
                }

                return developers;

            } catch (Exception)
            {
                throw;
            }
        }
    }
}
