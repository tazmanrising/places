﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HashingSalting
{
    class CreateHash
    {

        public byte[] GenerateComplexHash()
        {

            using (var sha256 = SHA256.Create())
            {
                string password = "hovercraft";

                // step 1: you can use RNGCryptoServiceProvider for something worth using
                string salt = GenerateSalt();

                // step 2
                string hash =
                   Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(salt + password)));

                // step 3
                byte[] result = sha256.ComputeHash(Encoding.UTF8.GetBytes(salt + hash));

                // step 4
                for (int i = 0; i < 60000; i++)
                {
                    result =
                     sha256.ComputeHash(Encoding.UTF8.GetBytes(salt + Convert.ToBase64String(result)));
                }

                return result;
            }


            

        }

        public string GenerateSalt()
        {

            return "blah";

        }


    }
}
