using BioTech.ViewModels;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Helpers
{
    public static class CustomSortingHelper
    {
        public static void SortByContractNumberDashboardRegular(DataSourceRequest request, DataSourceResult obj)
        {
            for (int i = 0; i < request.Sorts.Count; i++)
            {
                if (request.Sorts[i].Member == "ContractNumber")
                {
                    if (request.Sorts[i].SortDirection == 0)
                    {
                        obj.Data = obj.Data.Cast<DashboardRegularVM>().ToList().OrderBy(p => Convert.ToInt32(p.ContractNumber.Split('-')[0])).ThenBy(p => Convert.ToInt32(p.ContractNumber.Split('-')[1]));
                    }
                    else
                    {
                        obj.Data = obj.Data.Cast<DashboardRegularVM>().ToList().OrderByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[0])).ThenByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[1]));
                    }
                }
            }
        }

        public static void SortByContractNumberDashboardExecutive(DataSourceRequest request, DataSourceResult obj)
        {
            for (int i = 0; i < request.Sorts.Count; i++)
            {
                if (request.Sorts[i].Member == "ContractNumber")
                {
                    if (request.Sorts[i].SortDirection == 0)
                    {
                        obj.Data = obj.Data.Cast<ExecutiveDashboardKendoVM>().ToList().OrderBy(p => Convert.ToInt32(p.ContractNumber.Split('-')[0])).ThenBy(p => Convert.ToInt32(p.ContractNumber.Split('-')[1]));
                    }
                    else
                    {
                        obj.Data = obj.Data.Cast<ExecutiveDashboardKendoVM>().ToList().OrderByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[0])).ThenByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[1]));
                    }
                }
            }
        }
        public static void SortByContractNumberProject(DataSourceRequest request, DataSourceResult obj)
        {
            for (int i = 0; i < request.Sorts.Count; i++)
            {
                if (request.Sorts[i].Member == "ContractNumber")
                {
                    if (request.Sorts[i].SortDirection == 0)
                    {
                        obj.Data = obj.Data.Cast<ProjectFlatKendoGrid>().ToList().OrderBy(p => Convert.ToInt32(p.ContractNumber.Split('-')[0])).ThenBy(p => Convert.ToInt32(p.ContractNumber.Split('-')[1]));
                    }
                    else
                    {
                        obj.Data = obj.Data.Cast<ProjectFlatKendoGrid>().ToList().OrderByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[0])).ThenByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[1]));
                    }
                }
            }
        }

        public static void SortByContractNumberProposal(DataSourceRequest request, DataSourceResult obj)
        {
            for (int i = 0; i < request.Sorts.Count; i++)
            {
                if (request.Sorts[i].Member == "ContractNumber")
                {
                    if (request.Sorts[i].SortDirection == 0)
                    {
                        obj.Data = obj.Data.Cast<ProposalViewModel>().ToList().OrderBy(p => Convert.ToInt32(p.ContractNumber.Split('-')[0])).ThenBy(p => Convert.ToInt32(p.ContractNumber.Split('-')[1]));
                    }
                    else
                    {
                        obj.Data = obj.Data.Cast<ProposalViewModel>().ToList().OrderByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[0])).ThenByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[1]));
                    }
                }
            }
        }
        public static void SortByProjectNumber(DataSourceRequest request, DataSourceResult obj)
        {
            for (int i = 0; i < request.Sorts.Count; i++)
            {
                if (request.Sorts[i].Member == "ProjectNumber")
                {
                    if (request.Sorts[i].SortDirection == 0)
                    {
                        obj.Data = obj.Data.Cast<ProjectFlatKendoGrid>().ToList().OrderBy(p => Convert.ToInt32(p.ContractNumber.Split('-')[0])).ThenBy(p => Convert.ToInt32(p.ContractNumber.Split('-')[1]));
                    }
                    else
                    {
                        obj.Data = obj.Data.Cast<ProjectFlatKendoGrid>().ToList().OrderByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[0])).ThenByDescending(p => Convert.ToInt32(p.ContractNumber.Split('-')[1]));
                    }
                }
            }
        }    

    }
}
