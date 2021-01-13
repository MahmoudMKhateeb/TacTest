using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.DataExporting;
using TACHYON.Dto;
using TACHYON.Trucks;

namespace TACHYON.Waybills
{
    public class WaybillsAppService : TACHYONAppServiceBase
    {
        private readonly PdfExporterBase _pdfExporterBase;
        private readonly IRepository<Truck, Guid> _trucksRepository;

        public WaybillsAppService(PdfExporterBase pdfExporterBase, IRepository<Truck, Guid> trucksRepository)
        {
            _pdfExporterBase = pdfExporterBase;
            _trucksRepository = trucksRepository;
        }

        public FileDto GetPdf()
        {
            var reportPath = "/Waybills/Reports/Single_Drop_Waybill.rdlc";
            //var reportPath = "/Waybills/Reports/Multiple_Drop_Waybill.rdlc";
            //var reportPath = "/Waybills/Reports/Master_Waybill.rdlc";

            var list = _trucksRepository.GetAll()
                .Select(x => new
                {
                    x.ModelName,
                    x.ModelYear,
                    x.PlateNumber
                }).ToList();

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();

            names.Add("DataSet1");
            data.Add(list);

            return _pdfExporterBase.CreateRdlcPdfPackageFromList("test", reportPath, names, data);
        }
    }
}
