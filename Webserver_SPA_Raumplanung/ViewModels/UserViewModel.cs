using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DbRaumplanung.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AspNetCoreVueStarter.ViewModels
{
    public class UserViewModel: User
    {
        public List<Role> Roles { get; set; } = new List<Role>();
        public IList<long> SupportGroupIds { get; set; }
    }
    public class UserVMProfile : Profile
    {
        public UserVMProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.SupportGroupIds, opt => opt.MapFrom(src => src.SupplierGroups.Select(e => e.Id)));

            CreateMap<UserViewModel, User>();
        }
    }

    public class Role
    {
        [JsonIgnore]
        public string AdDescription { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool HasRole(Role role)
        {
            return role.Level >= Level;
        }
    }
}
