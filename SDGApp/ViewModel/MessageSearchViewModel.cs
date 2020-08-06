﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDGApp.ViewModel
{
    public class MessageSearchViewModel
    {
        public int UserID { get; set; }
        public string Picture { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

    }
}