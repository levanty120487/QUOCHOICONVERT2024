using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJCodeUI_M1.Models
{
    public class Attachments
    {
        /// <summary>
        /// This class was created for testing purposes only,
        /// the data may be inaccurate and meaningless.
        /// </summary>
        /// 
        public string Name { get; set; }
        public string Url { get; set; }
        public string FileServer { get; set; }
        public string FileName { get; set; }
        public string TitleName { get; set; }
        public byte[] DataFile { get; set; }

        public Attachments()
        {

        }
    }
}
