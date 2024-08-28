using TACHYON.Actors;

using System;
using Abp.Application.Services.Dto;
using TACHYON.Documents.DocumentFiles.Dtos;

namespace TACHYON.Actors.Dtos
{
    public class GetActorByPurchNoCDto 
    {
     public SalesOfficeTypeEnum Salesoffice { get; set; }
    public string Street { get; set; }
    public string PostalCode { get; set; }
    public string Phone { get; set; }
    public string Vatregisteration { get; set; }
    public string Registeration { get; set; }
    public string Lastname { get; set; }
    public string Firstname { get; set; }
    public string District { get; set; }
    public ActorTypesEnum Dischannel { get; set; }
    public string City { get; set; }
    public string BuildingCode { get; set; }


    }
}