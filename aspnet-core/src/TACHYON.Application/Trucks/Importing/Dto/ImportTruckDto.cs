using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Trucks.Dtos;

namespace TACHYON.Trucks.Importing.Dto
{
    public class ImportTruckDto
    {
        /// <summary>
        /// is used for create truck required documents from excel data
        /// </summary>
        public List<ImportTruckDocumentFileDto> ImportTruckDocumentFileDtos { get; set; }

        public string PlateNumber { get; set; }


        public string ModelName { get; set; }


        public string ModelYear { get; set; }



        public bool IsAttachable { get; set; }


        [StringLength(TruckConsts.MaxNoteLength, MinimumLength = TruckConsts.MinNoteLength)]
        public string Note { get; set; }

        public long? TruckStatusId { get; set; }

        public long? Driver1UserId { get; set; }

        public string Capacity { get; set; }


        #region Truck Categories

        public virtual int? TransportTypeId { get; set; }

        public virtual long? TrucksTypeId { get; set; }

        public virtual int? CapacityId { get; set; }

        #endregion

        /// <summary>
        /// Can be set when reading data from excel or when importing truck
        /// </summary>
        public string Exception { get; set; }

        public bool CanBeImported()
        {
            return string.IsNullOrEmpty(Exception);
        }

    }
}
