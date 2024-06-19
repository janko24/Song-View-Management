using System.ComponentModel.DataAnnotations;

namespace ImageSender.Models
{
    public class FolderFiles
    {
        //>>Create a boolean function with 2 arguments one is emailid and another is folder path
        //>>when function runs,send mail with all the image files present in the given folder and send it to given emailId
        //>>if mail with all the files send successfully then return true else false.
        //>>Also create entity tbl_FolderFiles with fields Id(Primarykey),EmailId,FiolderPath and IsmailSend(bool)

        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email ID")]
        public string EmailID { get; set; }

        [Required]
        [Display(Name = "Folder Path")]
        public string FolderPath { get; set; }

        [Display(Name = "Email Sent")]
        public bool IsMailSend { get; set; }
    }
}
