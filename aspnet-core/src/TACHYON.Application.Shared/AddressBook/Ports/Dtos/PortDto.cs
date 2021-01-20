
using System;
using Abp.Application.Services.Dto;
using NetTopologySuite.Geometries;

namespace TACHYON.AddressBook.Ports.Dtos
{
    public class PortDto : EntityDto<long>
    {
		public string Name { get; set; }

		public string Adress { get; set; }

		public Point Location { get; set; }

		 public int CityId { get; set; }

		 
    }
}