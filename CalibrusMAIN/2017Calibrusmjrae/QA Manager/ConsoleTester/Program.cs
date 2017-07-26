using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Collections;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {



            var ldap = new LDAPStuff();

            // example
            //LDAP://192.168.0.146/CN=USERS,DC=capp,DC=net

            //var x = ldap.GetCurrentDomainPath();

            //"LDAP://DC=calibrus,DC=lcl"


            //LDAP://DC=|SERVER NAME|[,DC=|EXTENSION|]


            //ldap.GetAllUsers();

            // This will list ALL the properties from AD (between 200 and 800..or more)
            // If someone has a solution for non AD servers please post it!


            ldap.GetAllProperties();




            ldap.GetAdditionalUserInfo();

            Console.ReadLine();



            //var directoryEntry = new DirectoryEntry("LDAP://capp.net");
            //directoryEntry.Username = @"capp\dhr2";
            //directoryEntry.Password = "admin@12345";
            //var directorySearcher = new DirectorySearcher(directoryEntry);


            var b = "adfaf";

            //var qa = new QA_Report_Service.QA_Builder();
            //qa.GetAllCalls();



        }

    }






    public class LDAPStuff
    {


        public void GetAllProperties()
        {
            //List<User> users = new List<User>();
            try
            {

                //"LDAP://DC=calibrus,DC=lcl"

                DirectoryEntry root = new DirectoryEntry("LDAP://DC=calibrus,DC=lcl");
                //root = new DirectoryEntry("LDAP://" + root.Properties["defaultNamingContext"][0]);

                DirectorySearcher search = new DirectorySearcher(root);
                search.Filter = "(&(objectClass=user)(objectCategory=person))";

                //displayname: Kendra Jackson;
                //samaccountname: kj9729;
                //mail: kj9729 @calibrus.com;
                //adspath: LDAP://CN=Kendra Jackson,OU=Agents,DC=calibrus,DC=lcl;


                //search.PropertiesToLoad.Add("samaccountname");
                //search.PropertiesToLoad.Add("displayname");
                //search.PropertiesToLoad.Add("mail");

                //search.PropertiesToLoad.Add(String.Empty);

                SearchResultCollection results = search.FindAll();
                if (results != null)
                {
                    foreach (SearchResult result in results)
                    {
                        foreach (DictionaryEntry property in result.Properties)
                        {
                            Console.Write(property.Key + ": ");
                            foreach (var val in (property.Value as ResultPropertyValueCollection))
                            {
                                Console.Write(val + "; ");
                            }
                            Console.WriteLine("");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }





        public void GetAllUsers()
        {
            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new
                  DirectoryEntry(GetCurrentDomainPath());

            ds = new DirectorySearcher(de);
            ds.Filter = "(&(objectCategory=User)(objectClass=person))";

            results = ds.FindAll();

            foreach (SearchResult sr in results)
            {
                // Using the index zero (0) is required!
                //Debug.WriteLine(sr.Properties["name"][0].ToString());
                Console.WriteLine(sr.Properties["name"][0].ToString());
            }
        }

        public void GetAdditionalUserInfo()
        {
            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new
                  DirectoryEntry(GetCurrentDomainPath());

            ds = new DirectorySearcher(de);
            // Full Name
            ds.PropertiesToLoad.Add("name");
            // Email Address
            ds.PropertiesToLoad.Add("mail");
            // First Name
            ds.PropertiesToLoad.Add("givenname");
            // Last Name (Surname)
            ds.PropertiesToLoad.Add("sn");
            // Login Name
            ds.PropertiesToLoad.Add("userPrincipalName");
            // Distinguished Name
            ds.PropertiesToLoad.Add("distinguishedName");

            //username
            ds.PropertiesToLoad.Add("username");

            ds.Filter = "(&(objectCategory=User)(objectClass=person))";

            results = ds.FindAll();

            foreach (SearchResult sr in results)
            {
                if (sr.Properties["name"].Count > 0)
                    Console.WriteLine(sr.Properties["name"][0].ToString());
                // If not filled in, then you will get an error
                if (sr.Properties["mail"].Count > 0)
                    Console.WriteLine(sr.Properties["mail"][0].ToString());
                if (sr.Properties["givenname"].Count > 0)
                    Console.WriteLine(
                         sr.Properties["givenname"][0].ToString());
                if (sr.Properties["sn"].Count > 0)
                    Console.WriteLine(sr.Properties["sn"][0].ToString());
                if (sr.Properties["userPrincipalName"].Count > 0)
                    Console.WriteLine(
                         sr.Properties["userPrincipalName"][0].ToString());
                if (sr.Properties["distinguishedName"].Count > 0)
                    Console.WriteLine(
                         sr.Properties["distinguishedName"][0].ToString());

                if(sr.Properties["username"].Count > 0)
                {
                    Console.WriteLine(sr.Properties["username"][0].ToString());
                }

            }
        }


        public string GetCurrentDomainPath()
        {
            DirectoryEntry de = new DirectoryEntry("LDAP://RootDSE");

            return "LDAP://" +
               de.Properties["defaultNamingContext"][0].
                   ToString();
        }

    }

}
