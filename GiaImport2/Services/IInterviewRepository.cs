using GiaImport2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiaImport2.Services
{
    public interface IInterviewRepository
    {
        Task<IEnumerable<string>> GetExamDates();
        Task<List<Governmentinfo>> GetParticipantsExamsData(string examDate);
    }
}
