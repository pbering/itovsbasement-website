﻿using System;

namespace WebApp.Data.Markdown
{
    public class InvalidFieldException : Exception
    {
        public InvalidFieldException()
        {
        }

        public InvalidFieldException(string message) : base(message)
        {
        }

        public InvalidFieldException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}