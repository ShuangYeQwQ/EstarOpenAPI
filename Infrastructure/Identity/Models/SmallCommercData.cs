using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Models
{
    public class SmallCommercData
    {
        public float zip3 { get; set; }
        public float state { get; set; }
        public float industry_naics { get; set; }
        public float years_in_business { get; set; }
        public float gross_revenue_usd { get; set; }
        public float num_employees { get; set; }
        public float payroll_usd { get; set; }
        public float prior_claims_5yr { get; set; }
        public float loss_ratio_5yr { get; set; }
        public float location_sqft { get; set; }
        public float num_locations { get; set; }
        public float sprinklers { get; set; }
        public float burglar_alarm { get; set; }
        public float hours_of_operation_per_week { get; set; }
        public float coverage_property_limit { get; set; }
        public float coverage_bi_limit { get; set; }
        public float coverage_gl_each_occur { get; set; }
        public float coverage_gl_aggregate { get; set; }
        public float deductible_property { get; set; }
        public float deductible_gl { get; set; }
    }
}
