using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.ModelViews.DrugModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Contract.Service.Interface
{
    public interface IDrugService
    {
        Task<(IEnumerable<DrugModelView> drugs, int totalElement, int totalPage)> GetAllDrugsPaginatedAsync(int page, int pageSize);
        Task<Drug> GetDrugByIdAsync(string id);
        Task<string> CreateDrugAsync(DrugModelView drug);
        Task<bool> DeleteDrugAsync(string id);
        Task<(IEnumerable<DrugModelView> drugs, int totalElement, int totalPage)> SearchDrugsAsync(string type, string value, int page, int pageSize);
        Task<IEnumerable<Drug>> GetRelatedDrugsAsync(string id, string type);  // ingredient | company
        Task IncrementSearchCountAsync(string id);
        Task<bool> UpdateDrugAsync(string id, UpdateDrugModelView updatedDrug);
        Task<IEnumerable<DrugModelView>> GetTopDrugsByTypeAsync(string type); // new | withdrawn | searched
        Task<IEnumerable<object>> GetTopCompaniesByDrugsAsync();
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< Updated upstream
=======
        Task<(IEnumerable<DrugModelView> drugs, int totalElement, int totalPage)> FilterByDrugGroupAsync(string group, int page, int pageSize);
        Task<IEnumerable<string>> GetAllCompaniesAsync();


>>>>>>> Stashed changes
=======

>>>>>>> 23c07a1f76d014faf8df54e413d12f4cac51d327
=======

>>>>>>> 23c07a1f76d014faf8df54e413d12f4cac51d327
    }
}
