﻿using BugTrackerMVC.Data;
using BugTrackerMVC.Models.Enums;
using BugTrackerMVC.Models;
using BugTrackerMVC.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTrackerMVC.Services
{
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _btRolesService;
        private readonly IBTProjectService _btProjectService;

        public BTTicketService(ApplicationDbContext context, 
                               IBTRolesService btRolesService,
                               IBTProjectService btProjectService)
        {
            _context = context;
            _btRolesService = btRolesService;
            _btProjectService = btProjectService;
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyIdAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Tickets
                                                     .Where(t => t.Project!.CompanyId == companyId)
                                                     .Include(t => t.Project)
                                                        .ThenInclude(t => t!.Company)
                                                     .Include(t => t.TicketPriority)
                                                     .Include(t => t.TicketType)
                                                     .Include(t => t.TicketStatus)
                                                     .Include(t => t.DeveloperUser)
                                                     .Include(t => t.SubmitterUser)
                                                     .OrderByDescending(t => t.TicketStatus)
                                                        .ThenByDescending(t => t.Archived)
                                                        .ThenByDescending(t => t.Created)
                                                     .ToListAsync();

                return tickets;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId, int companyId)
        {
            try
            {
                Ticket? ticket = await _context.Tickets
                                               .Include(t => t.Project)
                                                    .ThenInclude(p => p!.Company)
                                               .Include(t => t.TicketPriority)
                                               .Include(t => t.TicketType)
                                               .Include(t => t.TicketStatus)
                                               .Include(t => t.Comments)
                                               .Include(t => t.Attachments)
                                               .Include(t => t.History)
                                               .Include(t => t.DeveloperUser)
                                               .Include(t => t.SubmitterUser)
                                               .FirstOrDefaultAsync(t => t.Id == ticketId
                                                                    && t.SubmitterUser!.CompanyId == companyId);

                return ticket!;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            try
            {
                // set status to Resolved
                ticket.TicketStatusId = (await GetTicketStatusesAsync()).FirstOrDefault(s => s.Name == nameof(BTTicketStatuses.Resolved))!.Id;

                // set Archived property to true
                ticket.Archived = true;

                await UpdateTicketAsync(ticket);

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task RestoreTicketAsync(Ticket ticket)
        {
            try
            {
                // set Archived property to false
                ticket.Archived = false;

                await UpdateTicketAsync(ticket);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<TicketPriority>> GetTicketPrioritiesAsync()
        {
            try
            {
                IEnumerable<TicketPriority> ticketPriorities = await _context.TicketPriorities
                                                                        .ToListAsync();

                return ticketPriorities;

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<TicketStatus>> GetTicketStatusesAsync()
        {
            try
            {
                IEnumerable<TicketStatus> ticketStatus = await _context.TicketStatuses
                                                                        .ToListAsync();

                return ticketStatus;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<TicketType>> GetTicketTypesAsync()
        {
            try
            {
                IEnumerable<TicketType> ticketTypes = await _context.TicketTypes
                                                                        .ToListAsync();

                return ticketTypes;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            try
            {
                IEnumerable<Project> projects = await _context.Projects
                                                              .ToListAsync();

                return projects;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetUsersAsync()
        {
            try
            {
                List<BTUser> users = await _context.Users
                                             .ToListAsync();

                return users;

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task<BTUser> GetDeveloperAsync(int ticketId, int companyId)
        {
            try
            {
                Ticket? ticket = await GetTicketByIdAsync(ticketId, companyId);

                // if no developer, return null
                if (ticket.DeveloperUserId == null)
                {
                    return null!;
                }
                // otherwise, return developer
                else
                {
                    return ticket.DeveloperUser!;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AssignDeveloperAsync(int ticketId, string userId, int companyId)
        {
            try
            {
                Ticket? ticket = await GetTicketByIdAsync(ticketId, companyId);

                // if ticket status is new
                if (ticket.TicketStatusId == (await GetTicketStatusesAsync()).FirstOrDefault(s => s.Name == nameof(BTTicketStatuses.New))!.Id) {

                    // change to development
                    ticket.TicketStatusId = (await GetTicketStatusesAsync()).FirstOrDefault(s => s.Name == nameof(BTTicketStatuses.Development))!.Id;
                }

                ticket.DeveloperUserId = userId;

                await UpdateTicketAsync(ticket);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment)
        {
            try
            {
                await _context.AddAsync(ticketAttachment);
                await _context.SaveChangesAsync();

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task<TicketAttachment> GetTicketAttachmentsByIdAsync(int ticketAttachmentId)
        {
            try
            {
                TicketAttachment? ticketAttachment = await _context.TicketAttachments
                                                                  .Include(t => t.BTUser)
                                                                  .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);

                return ticketAttachment!;

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task AddCommentAsync(TicketComment ticketComment)
        {
            try
            {
                _context.Add(ticketComment);
                await _context.SaveChangesAsync();

            } catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            BTUser? btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            List<Ticket>? tickets = (await _btProjectService.GetAllProjectsByCompanyIdAsync(companyId))
                                                            .Where(p => p.Archived == false)
                                                            .SelectMany(p => p.Tickets!)
                                                            .Where(t => t.Archived == false | t.ArchivedByProject == false)
                                                            .ToList();

            try
            {
                if (await _btRolesService.IsUserInRoleAsync(btUser!, nameof(BTRoles.Admin)))
                {
                    return tickets;
                }
                else if (await _btRolesService.IsUserInRoleAsync(btUser!, nameof(BTRoles.Developer)))
                {
                    return tickets.Where(t => t.DeveloperUserId == userId || t.SubmitterUserId == userId)
                                  .ToList();
                }
                else if (await _btRolesService.IsUserInRoleAsync(btUser!, nameof(BTRoles.Submitter)))
                {
                    return tickets.Where(t => t.SubmitterUserId == userId)
                                  .ToList();
                }
                else if (await _btRolesService.IsUserInRoleAsync(btUser!, nameof(BTRoles.ProjectManager)))
                {
                    List<Ticket>? projectTickets = (await _btProjectService.GetUserProjectsAsync(userId))!.SelectMany(t => t.Tickets!)
                                                                                                          .Where(t => t.Archived | t.ArchivedByProject)
                                                                                                          .ToList();
                    List<Ticket>? submittedTickets = tickets.Where(t => t.SubmitterUserId == userId)
                                                            .ToList();

                    return tickets = projectTickets.Concat(submittedTickets).ToList();
                }

                return tickets;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId, int companyId)
        {
            Ticket? ticket = await _context.Tickets
                                               .Include(t => t.Project)
                                                    .ThenInclude(p => p!.Company)
                                               .Include(t => t.TicketPriority)
                                               .Include(t => t.TicketType)
                                               .Include(t => t.TicketStatus)
                                               .Include(t => t.Comments)
                                               .Include(t => t.Attachments)
                                               .Include(t => t.History)
                                               .Include(t => t.DeveloperUser)
                                               .Include(t => t.SubmitterUser)
                                               .AsNoTracking()
                                               .FirstOrDefaultAsync(t => t.Id == ticketId && t.Project!.CompanyId == companyId && t.Archived == false);

            return ticket!;
        }
    }
}
