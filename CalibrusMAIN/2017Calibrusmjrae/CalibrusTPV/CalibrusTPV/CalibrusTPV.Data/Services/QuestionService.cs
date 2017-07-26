using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalibrusTPV.Data.Models;
using CalibrusTPV.Data.ScriptsDb;

namespace CalibrusTPV.Data.Services
{
    public class QuestionService
    {


        public List<Question> GetAllQuestions()
        {

            using (var scriptsContext = new ScriptsContext())
            {

                var queryQuestions = scriptsContext.Questions
                    //.Where(t => t.Active == true)
                    .AsEnumerable()
                    .Select(t => new Question()
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        Verbiage = t.Verbiage,
                        VerbiageSpanish = t.VerbiageSpanish,
                        Active = t.Active
                    }).ToList();

                return queryQuestions;

            }

        }

        public int CreateQuestion()
        {
            // todo  probably use 2 different models ,  need to insert into 3 tables?
            //  question , bridge entity, and the directive  table  
            // create a stored proc ...   use tablemapping

            // List<Title> t;
            //   using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            //   {
            //       TitleRepository repo = new TitleRepository(ctx);
            //       t = repo.Filter(x => x.IsActive == true);
            //       ctx.SaveChanges();
            //   }

            //   return t;
            return 1;

        }




    }

}
