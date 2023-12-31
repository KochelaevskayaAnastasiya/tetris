using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using tetris.Add_classes;
using static System.Net.Mime.MediaTypeNames;

namespace tetris.Pages
{
    public class FiguresModel : PageModel
    {
        private DataBase database = new DataBase();
        public List<Figure> figures = new List<Figure>();

        public int figure_count = 0;
        public int k = 0;

        public List<List<int>> fig = new List<List<int>>();
        public List<String> fig_str = new List<String>();
        
        public string alert_text = "";
        public void OnGet(string alert_text_str)
        {
            alert_text = alert_text_str;
            string queryString = "SELECT Structure FROM [Shape];";
            //string queryString = "SELECT Structure FROM [Figures];";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            database.openConnection();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                figures.Add(new Figure(reader[0].ToString()));
            }
            figure_count = figures.Count;
            reader.Close();
            database.closeConnection();

            for (int i = 0; i < figure_count; i++)
            {
                List<int> a = new List<int>();
                string str = figures[i].Structure;
                for (int j = 0; j < 16; j++)
                {
                    a.Add(int.Parse(Convert.ToString(str[j])));

                }
                fig.Add(a);
                fig_str.Add(str);
            }

        }

        public int CountFiguBD()
        {
            string queryString = "SELECT COUNT(*) FROM [Shape];";

            SqlCommand command = new SqlCommand(queryString, database.getConnection());
            database.openConnection();
            SqlDataReader reader = command.ExecuteReader();
            int l = 0;
            while (reader.Read())
            {
                l = int.Parse(reader[0].ToString());
            }
            reader.Close();
            database.closeConnection();
            return l;
        }

        public void DeleteSet(int k)
        {
            string queryString2 = "DELETE FROM SetOfShapes WHERE Shape_Id = "+k+";";
            database.openConnection();
            SqlCommand command_insert2 = new SqlCommand(queryString2, database.getConnection());
            int number2 = command_insert2.ExecuteNonQuery();
            Console.WriteLine("�������: {0} ������ � ��������", number2);
            database.closeConnection();
        }

        [HttpPost]
        public IActionResult OnPost()
        {
            string s = Request.Form["kkk"];
            string[] figures_new = s.Split(',');
            List<String> fig_str2 = new List<String>();
            for(int i = 0; i < figures_new.Length; i++)
            {
                if (CheckFigure.CheckIntegrity(figures_new[i]))
                {
                    fig_str2.Add(figures_new[i]);
                }
            }
            int noIntegrity = figures_new.Length-fig_str2.Count;

            string[] res = CheckFigure.CheckFigures(fig_str2.ToArray());

            int noUnick = fig_str2.Count - res.Length;

            //---------------------------------
            int k = 0;
            int coun = CountFiguBD();
            if (coun < res.Length)
            {
                for (int i = 0; i < coun; i++)
                {
                    k = i + 1;
                    string queryString = "UPDATE Shape SET Structure = '" + res[i] + "' WHERE Shape_Id = '" + k + "';";
                    database.openConnection();
                    SqlCommand command_insert = new SqlCommand(queryString, database.getConnection());
                    int number = command_insert.ExecuteNonQuery();
                    Console.WriteLine("��������: {0}", number);
                    database.closeConnection();
                }

                for (int j = coun; j < res.Length; j++)
                {
                    k = j + 1;
                    string queryString = "INSERT INTO[Shape] VALUES("+k+ ",'" + res[j] + "');";
                    database.openConnection();
                    SqlCommand command_insert = new SqlCommand(queryString, database.getConnection());
                    int number = command_insert.ExecuteNonQuery();
                    Console.WriteLine("���������: {0}", number);
                    database.closeConnection();
                }
            }
            else
            {
                for (int i = 0; i < res.Length; i++)
                {
                    k = i + 1;
                    string queryString = "UPDATE Shape SET Structure = '" + res[i] + "' WHERE Shape_Id = '" + k + "';";
                    database.openConnection();
                    SqlCommand command_insert = new SqlCommand(queryString, database.getConnection());
                    int number = command_insert.ExecuteNonQuery();
                    Console.WriteLine("��������: {0}", number);
                    database.closeConnection();
                }

                for (int j = res.Length; j < coun; j++)
                {
                    k = j + 1;
                    DeleteSet(k);
                    string queryString3 = "  DELETE FROM Shape WHERE  Shape_Id = "+k+";";
                    database.openConnection();
                    SqlCommand command_insert3 = new SqlCommand(queryString3, database.getConnection());
                    int number3 = command_insert3.ExecuteNonQuery();
                    Console.WriteLine("��������: {0}", number3);
                    database.closeConnection();
                }
            }
            alert_text = "";

            if (noIntegrity!=0 || noUnick != 0)
            {
                alert_text = "��������� ������ �� ���� �������:\n";
                if (noIntegrity != 0)
                {
                    alert_text += "�� ��������: " + noIntegrity;
                }
                if (noUnick != 0)
                {
                    alert_text += "\n�� ���������: " + noUnick;
                }
            }
            Console.WriteLine("�� ��������:"+ noIntegrity+"\n�� ���������:"+ noUnick);

            return RedirectToPage("Figures", new { alert_text_str = alert_text });
        }
    }
}
