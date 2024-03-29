﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTrackerMVC.Models
{
    // file attachments for a ticket
    public class TicketAttachment
    {
        public int Id { get; set; }

        [Display(Name = "File Description")]
        [StringLength(1000)]
        public string? Description { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Added")]
        public DateTimeOffset Created { get; set; }

        [NotMapped]
        [DisplayName("Select a File")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf" } )]
        public IFormFile? FormFile { get; set; }

        [Display(Name = "Ticket Files")]
        public byte[]? FileData { get; set; }

        [Display(Name = "File Extension")]
        public string? FileType { get; set; }

        [Display(Name = "File Name")]
        public string? FileName { get; set; }

        
        
        // ---------------- foreign keys ----------------

        public int TicketId { get; set; }

        [Required]
        public string? BTUserId { get; set; }

        // ---------------- foreign keys ----------------

        // --------------- nav properties ---------------
        public virtual Ticket? Ticket { get; set; }

        [Display(Name = "Team Member")]
        public virtual BTUser? BTUser { get; set; }

        // --------------- nav properties ---------------
    }
}
