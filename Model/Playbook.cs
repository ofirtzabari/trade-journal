using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TradeApp.Model
{
    public class Playbook
    {
        [Key] // המפתח הראשי הוא שם הפלייבוק
        public string Name { get; set; }

        public List<string> Criteria { get; set; } = new List<string>();

        public Playbook(string name, List<string> criteria)
        {
            Name = name;
            Criteria = criteria;
        }
    }
}
