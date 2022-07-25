﻿using Microsoft.EntityFrameworkCore;

namespace PostCalendarAPI.Models
{
    public class ICSInfo
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Semester { get; set; }
        public string CreatedDateTimeString { get; set; }
        public byte[] Data { get; set; }

        public ICSInfo(int id, string createdDateTimeString, byte[] data, string userName, string semester)
        {
            ID = id;
            CreatedDateTimeString = createdDateTimeString;
            Data = data;
            UserName = userName;
            Semester = semester;
        }

        public ICSInfo(DateTime createdDateTime, byte[] data, string userName, string semester)
        {
            CreatedDateTimeString = createdDateTime.ToString("s");
            UserName = userName;
            Semester = semester;

            Data = data;
        }
    }

    public class ICSInfoContext : DbContext
    {
        public ICSInfoContext()
        {
            Database.EnsureCreated();
        }

        public ICSInfoContext(DbContextOptions<ICSInfoContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ICSInfo> ICSInfos { get; set; } = null!;
    }
}