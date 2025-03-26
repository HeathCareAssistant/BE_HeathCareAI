using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
namespace HealthyCareAssistant.ModelViews.DrugModelViews
{


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DrugGroupType
    {
        [EnumMember(Value = "Tân dược")]
        TanDuoc,

        [EnumMember(Value = "Đông dược")]
        DongDuoc
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DrugCategoryType
    {
        [EnumMember(Value = "Thuốc kê đơn")]
        ThuocKeDon,

        [EnumMember(Value = "Thuốc không kê đơn")]
        ThuocKhongKeDon
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DrugStatusType
    {
        [EnumMember(Value = "Created")]
        Created,

        [EnumMember(Value = "Approved")]
        Approved,

        [EnumMember(Value = "Updated")]
        Updated,

        [EnumMember(Value = "Inactive")]
        Inactive
    }

    public class DrugFilterRequest
    {
        public DrugGroupType? Group { get; set; }
        public DrugCategoryType? Category { get; set; }
        public DrugStatusType? Status { get; set; }
    }

}
