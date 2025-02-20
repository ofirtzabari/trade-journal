using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradeApp.Model;
using TradeApp.Context;

namespace TradeApp.Data
{
    public static class PlaybookData
    {
        // ✅ הוספת פלייבוק חדש לבסיס הנתונים
        public static void AddPlaybookToDb(Playbook playbook)
        {
            using (var db = new PlaybookContext())
            {
                db.Playbooks.Add(playbook);
                db.SaveChanges();
            }
        }

        // ✅ שליפת כל הפלייבוקים מהדאטהבייס
        public static List<Playbook> GetPlaybookList()
        {
            using (var db = new PlaybookContext())
            {
                return db.Playbooks.ToList();
            }
        }

        // ✅ מחיקת פלייבוק על פי שם (מפתח ראשי)
        public static void DeletePlaybook(Playbook playbook)
        {
            using (var db = new PlaybookContext())
            {
                var playbookToDelete = db.Playbooks.FirstOrDefault(p => p.Name == playbook.Name);
                if (playbookToDelete != null)
                {
                    db.Playbooks.Remove(playbookToDelete);
                    db.SaveChanges();
                }
            }
        }

        // ✅ עדכון פלייבוק קיים
        public static void UpdatePlaybookInDb(Playbook playbook)
        {
            using (var db = new PlaybookContext())
            {
                db.Playbooks.Update(playbook);
                db.SaveChanges();
            }
        }
    }
}
