using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.MedicineCabinetModelViews
{
    public class CabinetDrugDetailView
    {
        public string DrugId { get; set; }
        public string TenThuoc { get; set; }
        public string HoatChat { get; set; }
        public string PhanLoai { get; set; }
        public string CongTySx { get; set; }
        public double? GiaKeKhai { get; set; }
        public string Note { get; set; } // Ghi chú thuốc trong tủ thuốc
    }
}
