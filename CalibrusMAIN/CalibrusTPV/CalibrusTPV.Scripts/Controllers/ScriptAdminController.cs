using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CalibrusTPV.Data.Models;
using CalibrusTPV.Data.Services;

namespace CalibrusTPV.Scripts.Controllers
{
    public class ScriptAdminController : ApiController
    {

        [HttpGet]
        [Route("api/GetAllQuestions")]
        public IHttpActionResult GetQuestions()
        {

            var questionService = new QuestionService();
            List<Question> questions = new List<Question>();
            questions = questionService.GetAllQuestions();


            return Ok(questions);

        }

    }

}
