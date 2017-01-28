using System;
using System.Threading.Tasks;

namespace GymTracker.Services
{
    public interface ISessionsSerivce
    {
        Task RecordSession(DateTimeOffset sessionDate);
    }
}