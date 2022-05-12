using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.AddressBook.Ports.Dtos
{
    public class GetPortForEditOutput
    {
        public CreateOrEditPortDto Port { get; set; }

        public string CityDisplayName { get; set; }
    }
}