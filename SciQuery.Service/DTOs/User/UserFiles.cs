using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciQuery.Service.DTOs.User
{
    public class UserFiles
    {
        public byte[] Bytes { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public UserFiles()
        {

        }
        public UserFiles(byte[] bytes, string contentType, string fileName)
        {
            Bytes = bytes;
            ContentType = contentType;
            FileName = fileName;
        }
    }
}
