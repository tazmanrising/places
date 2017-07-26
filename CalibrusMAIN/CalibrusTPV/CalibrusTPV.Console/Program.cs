using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalibrusTPV.Data.Models;
using CalibrusTPV.Data.Services;

namespace CalibrusTPV.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            var questionService = new QuestionService();
            List<Question> questions = new List<Question>();
            questions = questionService.GetAllQuestions();

            System.Console.ReadLine();




        }

    }

}
