﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareEng
{
    

    public class Error
    {
        //---------------------------
        //CONSTANTS
        public const int SUCCESS = 0;
        public const int SHIT_JUST_GOT_REAL = 9001;
        //---------------------------
        //Vars
        public int id;
        public string description;
        //---------------------------
        //Default to success, of course!
        public Error()
        {
            id = 0;
            description = "";
        }


    }

}
